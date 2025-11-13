using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using TokenService = CoffeeCard.Library.Services.v2.TokenService;

namespace CoffeeCard.Tests.Unit.Services.v2
{
    public class TokenServiceTests : BaseUnitTests
    {
        [Fact(DisplayName = "GenerateMagicLink returns a link with a valid token for user")]
        public async Task GenerateMagicLink_ReturnsLinkWithValidTokenForUser()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            InitialContext.Users.Add(user);
            await InitialContext.SaveChangesAsync();

            var hashService = new HashService();
            var tokenService = new TokenService(InitialContext, hashService);

            // Act
            var result = await tokenService.GenerateMagicLinkToken(user);

            // Assert
            var assertionUser = await AssertionContext
                .Users.Include(u => u.Tokens)
                .FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            var hashedToken = hashService.Hash(result);
            Assert.Contains<Token>(assertionUser.Tokens, t => t.TokenHash == hashedToken);
            var token = assertionUser.Tokens.First(t => t.TokenHash == hashedToken);
            Assert.Equal(TokenType.MagicLink, token.Type);
            Assert.False(token.Revoked, "Token should not be revoked");
        }

        [Fact(DisplayName = "GenerateRefreshTokenAsync returns a valid token for user")]
        public async Task GenerateRefreshTokenAsync_ReturnsValidTokenForUser()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            InitialContext.Users.Add(user);
            await InitialContext.SaveChangesAsync();
            var hashService = new HashService();

            var tokenService = new TokenService(AssertionContext, hashService);

            // Act
            var result = await tokenService.GenerateRefreshTokenAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // We cannot assert on tokenHash since it has been hashed for security reasons. Hashing is tested elsewhere.
            var token = await AssertionContext.Tokens.FirstOrDefaultAsync();
            Assert.Equal(TokenType.Refresh, token.Type);
            Assert.False(token.Revoked, "Token should not be revoked");
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync throws exception if token does not exist")]
        public async Task GetValidTokenByHashAsync_ThrowsExceptionIfTokenDoesNotExist()
        {
            // Arrange
            var tokenService = new TokenService(AssertionContext, Mock.Of<IHashService>());

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                tokenService.GetValidTokenByHashAsync("token")
            );
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync throws exception if token is revoked")]
        public async Task GetValidTokenByHashAsync_ThrowsExceptionIfTokenIsRevoked()
        {
            // Arrange
            var token = TokenBuilder.Simple().Build();

            InitialContext.Tokens.Add(token);

            await InitialContext.SaveChangesAsync();

            var tokenService = new TokenService(AssertionContext, Mock.Of<IHashService>());

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                tokenService.GetValidTokenByHashAsync("token")
            );
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync throws exception if token has expired")]
        public async Task GetValidTokenByHashAsync_ThrowsExceptionIfTokenHasExpired()
        {
            // Arrange
            var token = TokenBuilder.Simple().WithExpires(DateTime.Now.AddDays(-1)).Build();
            InitialContext.Tokens.Add(token);

            await InitialContext.SaveChangesAsync();

            var tokenService = new TokenService(AssertionContext, Mock.Of<IHashService>());

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                tokenService.GetValidTokenByHashAsync("token")
            );
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync returns token by valid hash")]
        public async Task GetValidTokenByHashAsync_ReturnsTokenByValidHash()
        {
            // Arrange
            var token = TokenBuilder.Simple().Build();
            InitialContext.Tokens.Add(token);

            await InitialContext.SaveChangesAsync();

            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns(token.TokenHash);

            var tokenService = new TokenService(AssertionContext, hashService.Object);

            // Act & Assert
            var result = await tokenService.GetValidTokenByHashAsync(token.TokenHash);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result);
            Assert.False(result.Revoked);
            Assert.False(Token.Expired(result.Expires));
        }
    }
}
