using System;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Microsoft.Data.Sqlite;
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
            var token = TokenBuilder.Simple().WithType(TokenType.MagicLink).Build();
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

        [Fact(DisplayName = "GetValidTokenByHashAsync rejects a registration token")]
        public async Task GetValidTokenByHashAsync_RejectsRegistrationToken()
        {
            // Arrange
            var token = TokenBuilder.Simple().WithType(TokenType.Registration).Build();
            InitialContext.Tokens.Add(token);
            await InitialContext.SaveChangesAsync();

            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns(token.TokenHash);

            var tokenService = new TokenService(AssertionContext, hashService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                tokenService.GetValidTokenByHashAsync(token.TokenHash)
            );
        }

        [Fact(DisplayName = "GetValidTokenByHashAsync accepts a refresh token")]
        public async Task GetValidTokenByHashAsync_AcceptsRefreshToken()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();
            var token = TokenBuilder.Simple().WithType(TokenType.Refresh).WithUser(user).Build();
            InitialContext.Users.Add(user);
            InitialContext.Tokens.Add(token);
            await InitialContext.SaveChangesAsync();

            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns(token.TokenHash);

            var tokenService = new TokenService(AssertionContext, hashService.Object);

            // Act
            var result = await tokenService.GetValidTokenByHashAsync(token.TokenHash);

            // Assert
            Assert.Equal(TokenType.Refresh, result.Type);
            Assert.NotNull(result.User);
        }

        [Fact(
            DisplayName = "GenerateRegistrationTokenAsync stores the email and revokes older links"
        )]
        public async Task GenerateRegistrationTokenAsync_StoresEmailAndRevokesOlderLinks()
        {
            // Arrange
            const string email = "new@test.test";
            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashedToken");

            // GenerateRegistrationTokenAsync uses ExecuteUpdate, which the
            // in-memory provider used by this base class does not support.
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseSqlite(connection)
                .Options;

            await using var context = new CoffeeCardContext(
                options,
                new DatabaseSettings { SchemaName = "main" },
                new EnvironmentSettings
                {
                    EnvironmentType = EnvironmentType.Test,
                    MinAppVersion = "2.1.0",
                    DeploymentUrl = "test",
                }
            );
            await context.Database.EnsureCreatedAsync();

            var tokenService = new TokenService(context, hashService.Object);

            // Act
            await tokenService.GenerateRegistrationTokenAsync(email);
            await tokenService.GenerateRegistrationTokenAsync(email);

            // Assert
            context.ChangeTracker.Clear();
            var tokens = await context
                .Tokens.Where(t => t.Type == TokenType.Registration && t.Email == email)
                .ToListAsync();

            Assert.Equal(2, tokens.Count);
            Assert.Single(tokens.Where(t => !t.Revoked));
        }

        [Fact(DisplayName = "The database rejects a second active registration link for an email")]
        public async Task RegistrationTokenIndex_RejectsSecondActiveLinkForSameEmail()
        {
            // Arrange
            const string email = "new@test.test";

            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseSqlite(connection)
                .Options;

            await using var context = new CoffeeCardContext(
                options,
                new DatabaseSettings { SchemaName = "main" },
                new EnvironmentSettings
                {
                    EnvironmentType = EnvironmentType.Test,
                    MinAppVersion = "2.1.0",
                    DeploymentUrl = "test",
                }
            );
            await context.Database.EnsureCreatedAsync();

            context.Tokens.Add(new Token("first", TokenType.Registration) { Email = email });
            await context.SaveChangesAsync();

            // Act & Assert
            context.Tokens.Add(new Token("second", TokenType.Registration) { Email = email });
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact(DisplayName = "GetValidRegistrationTokenAsync rejects a token of another type")]
        public async Task GetValidRegistrationTokenAsync_RejectsTokenOfAnotherType()
        {
            // Arrange
            var token = TokenBuilder.Simple().WithType(TokenType.MagicLink).Build();
            InitialContext.Tokens.Add(token);
            await InitialContext.SaveChangesAsync();

            var hashService = new Mock<IHashService>();
            hashService.Setup(h => h.Hash(It.IsAny<string>())).Returns(token.TokenHash);

            var tokenService = new TokenService(AssertionContext, hashService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                tokenService.GetValidRegistrationTokenAsync(token.TokenHash)
            );
        }
    }
}
