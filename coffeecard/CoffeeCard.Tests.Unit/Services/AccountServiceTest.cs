using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class AccountServiceTest
    {
        [Fact(DisplayName = "RecoverUser given malformed token returns false")]
        public async Task RecoverUserGivenMalformedTokenReturnsFalse()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenMalformedTokenReturnsFalse));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings();

            var expectedResult = false;
            bool result;

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                    new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                    new Mock<IHttpContextAccessor>().Object);
                result = await accountService.RecoverUser("bogus", "3433");
            }

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token returns true")]
        public async Task RecoverUserGivenValidTokenReturnsTrue()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenValidTokenReturnsTrue));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings();

            var expectedResult = true;
            bool result;

            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> {claim};
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateToken("valid")).ReturnsAsync(true);

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var token = new Token("valid");
                var userTokens = new List<Token> {token};

                var user = new User {Tokens = userTokens, Email = "test@email.dk", Programme = new Programme()};
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                    new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                    new Mock<IHttpContextAccessor>().Object);

                result = await accountService.RecoverUser("valid", "3433");
            }

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token updates password and resets users tokens")]
        public async Task RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings();

            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> {claim};
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateToken("valid")).ReturnsAsync(true);

            var userPass = "not set";
            string newUserPass;
            ICollection<Token> newUserTokens;

            // Act
            await using (var context = new CoffeeCardContext(builder.Options, databaseSettings))
            {
                var token = new Token("valid");
                var userTokens = new List<Token> {token};

                var user = new User
                    {Tokens = userTokens, Email = "test@email.dk", Programme = new Programme(), Password = userPass};
                context.Add(user);
                context.SaveChanges();

                var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                    new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                    new Mock<IHttpContextAccessor>().Object);

                await accountService.RecoverUser("valid", "3433");

                var updatedUser = context.Users.FirstOrDefault(u => u.Email == user.Email);
                newUserPass = updatedUser.Password;
                newUserTokens = updatedUser.Tokens;
            }

            var expectedTokenCount = 0;

            // Assert
            Assert.NotEqual(newUserPass, userPass);
            Assert.Equal(newUserTokens.Count, expectedTokenCount);
        }
    }
}