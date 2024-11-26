using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using TokenService = CoffeeCard.Library.Services.v2.TokenService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class TokenServiceTests
    {
        private CoffeeCardContext CreateTestCoffeeCardContextWithName(string name)
        {
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(name);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            return new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
        }

        [Fact(DisplayName = "GenerateMagicLink returns a link with a valid token for user")]
        public async Task GenerateMagicLink_ReturnsLinkWithValidTokenForUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User"
            };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GenerateMagicLink_ReturnsLinkWithValidTokenForUser));

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var tokenService = new TokenService(context, Mock.Of<IHashService>());

            // Act
            var result = await tokenService.GenerateMagicLink(user);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains<Token>(user.Tokens, t => t.TokenHash == result);
            var token = user.Tokens.First(t => t.TokenHash == result);
            Assert.Equal(TokenType.MagicLink, token.Type);
            Assert.False(token.Revoked, "Token should not be revoked");
        }

        [Fact(DisplayName = "GenerateRefreshTokenAsync returns a valid token for user")]
        public async Task GenerateRefreshTokenAsync_ReturnsValidTokenForUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User"
            };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GenerateRefreshTokenAsync_ReturnsValidTokenForUser));

            context.Users.Add(user);
            await context.SaveChangesAsync();
            var hashService = new HashService();

            var tokenService = new TokenService(context, hashService);

            // Act
            var result = await tokenService.GenerateRefreshTokenAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // We cannot assert on tokenHash since it has been hashed for security reasons. Hashing is tested elsewhere.
            var token = user.Tokens.First();
            Assert.Equal(TokenType.Refresh, token.Type);
            Assert.False(token.Revoked, "Token should not be revoked");
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync throws exception if token does not exist")]
        public async Task GetValidTokenByHashAsync_ThrowsExceptionIfTokenDoesNotExist()
        {
            // Arrange
            using var context = CreateTestCoffeeCardContextWithName(nameof(GetValidTokenByHashAsync_ThrowsExceptionIfTokenDoesNotExist));

            var tokenService = new TokenService(context, Mock.Of<IHashService>());

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => tokenService.GetValidTokenByHashAsync("token"));
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync throws exception if token is revoked")]
        public async Task GetValidTokenByHashAsync_ThrowsExceptionIfTokenIsRevoked()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User"
            };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetValidTokenByHashAsync_ThrowsExceptionIfTokenIsRevoked));

            context.Users.Add(user);

            var token = new Token("token", TokenType.Refresh) { User = user };
            context.Tokens.Add(token);

            token.Revoked = true;
            await context.SaveChangesAsync();

            var tokenService = new TokenService(context, Mock.Of<IHashService>());

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => tokenService.GetValidTokenByHashAsync("token"));
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync throws exception if token has expired")]
        public async Task GetValidTokenByHashAsync_ThrowsExceptionIfTokenHasExpired()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User"
            };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetValidTokenByHashAsync_ThrowsExceptionIfTokenHasExpired));

            context.Users.Add(user);

            var token = new Token("token", TokenType.Refresh) { User = user, Expires = DateTime.Now.AddDays(-1) };
            context.Tokens.Add(token);

            await context.SaveChangesAsync();

            var tokenService = new TokenService(context, Mock.Of<IHashService>());

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => tokenService.GetValidTokenByHashAsync("token"));
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync returns token by valid hash")]
        public async Task GetValidTokenByHashAsync_ReturnsTokenByValidHash()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User"
            };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetValidTokenByHashAsync_ReturnsTokenByValidHash));

            context.Users.Add(user);

            var token = new Token("token", TokenType.Refresh) { User = user };
            context.Tokens.Add(token);

            await context.SaveChangesAsync();

            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns("token");

            var tokenService = new TokenService(context, hashService.Object);

            // Act & Assert
            var result = await tokenService.GetValidTokenByHashAsync("token");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result);
            Assert.False(result.Revoked);
            Assert.False(result.Expired());
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync invalidates users refresh tokens if token is invalid")]
        public async Task GetValidTokenByHashAsync_InvalidatesUsersRefreshTokensIfTokenIsInvalid()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                Name = "Test User"
            };

            using var context = CreateTestCoffeeCardContextWithName(nameof(GetValidTokenByHashAsync_InvalidatesUsersRefreshTokensIfTokenIsInvalid));

            context.Users.Add(user);

            var token = new Token("token", TokenType.Refresh) { User = user, Revoked = true };
            var refreshToken = new Token("refresh", TokenType.Refresh) { User = user };

            Token[] otherTokens =
            {
                new ("magicLink", TokenType.MagicLink) {User = user},
                new ("reset", TokenType.ResetPassword) {User = user},
            };

            context.Tokens.AddRange(token, refreshToken);
            context.Tokens.AddRange(otherTokens);

            await context.SaveChangesAsync();

            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns("token");

            var tokenService = new TokenService(context, hashService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => tokenService.GetValidTokenByHashAsync("token"));

            // Assert
            Assert.True(refreshToken.Revoked);
            foreach (var otherToken in otherTokens)
            {
                Assert.False(otherToken.Revoked);
            }
        }
    }
}
