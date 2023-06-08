using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class AccountServiceTest
    {
        private static User testuser => new User(
            email: "test@email.com",
            name: "name",
            password: "password",
            salt: "salt",
            programme: new Programme(fullName: "fullName", shortName: "shortName") { Id = 1 }
        );

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
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            var expectedResult = false;

            // Act
            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);
            var result = await accountService.RecoverUserAsync("bogus", "3433");

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
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            var token = new Token("valid");
            var user = testuser;
            user.Tokens = new List<Token> { token };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var claim = new Claim(ClaimTypes.Email, user.Email);
            var claims = new List<Claim> { claim };
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateTokenIsUnusedAsync("valid")).ReturnsAsync(true);

            // Act
            var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);

            var result = await accountService.RecoverUserAsync(token.TokenHash, "3433");

            // Assert
            Assert.True(result);
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
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> { claim };
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateTokenIsUnusedAsync("valid")).ReturnsAsync(true);

            var userPass = "not set";

            // Act
            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            var token = new Token("valid");
            var user = testuser;
            user.Email = "test@email.dk";
            user.Tokens = new List<Token> { token };
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);

            await accountService.RecoverUserAsync("valid", "3433");

            var updatedUser = context.Users.FirstOrDefault(u => u.Email == user.Email);
            var newUserPass = updatedUser?.Password;
            var newUserTokens = updatedUser?.Tokens;

            var expectedTokenCount = 0;

            // Assert
            Assert.NotEqual(newUserPass, userPass);
            Assert.Equal(newUserTokens?.Count, expectedTokenCount);
        }

        [Fact(DisplayName = "Login given valid credentials returns token")]
        public async Task LoginGivenValidCredentialsReturnsToken()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginGivenValidCredentialsReturnsToken));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            var user = testuser;
            user.IsVerified = true;

            var hasher = new Mock<IHashService>();
            hasher.Setup(h => h.Hash(user.Password + user.Salt)).Returns(user.Password);

            var expectedToken = "valid";

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>())).Returns(expectedToken);

            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var accountService = new AccountService(context, environmentSettings, tokenService.Object,
                new Mock<IEmailService>().Object, hasher.Object,
                new Mock<IHttpContextAccessor>().Object, loginLimiter.Object, loginLimiterSettings);

            var actualToken = accountService.Login(user.Email, user.Password, "2.1.0");

            // Assert
            Assert.Equal(expectedToken, actualToken);
        }

        [Fact(DisplayName = "LoginLimiter is called if limiter is enabled")]
        public async Task LoginRejectsAfterFiveFailedLogins()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginRejectsAfterFiveFailedLogins));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            var user = testuser;
            user.IsVerified = true;

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                loginLimiter.Object, loginLimiterSettings);

            //Attempts to login
            Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            // Assert
            loginLimiter.Verify(l => l.LoginAllowed(user), Times.Once);
        }

        [Fact(DisplayName = "LoginLimiter not called if limiter is disabled")]
        public async Task LoginLimiterNotCalledWhenDisabled()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginLimiterNotCalledWhenDisabled));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = false,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            var user = testuser;

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                loginLimiter.Object, loginLimiterSettings);

            //Attempts to login
            Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            // Assert
            loginLimiter.Verify(l => l.LoginAllowed(user), Times.Never);
        }

        [Fact(DisplayName = "Login throws exception when limit is reached and limiter is enabled")]
        public async Task LoginThrowsExceptionWhenLimitIsReached()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginThrowsExceptionWhenLimitIsReached));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };

            var user = testuser;
            user.IsVerified = true;

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            // Act
            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            //Attempts to login with the wrong credentials 
            Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            //Attempts to login a sixth time with the same credentials and captures the exception 
            var tooManyLoginsException =
                (ApiException)Record.Exception(() => accountService.Login(user.Email, wrongPass, "2.1.0"));
            // Assert
            var expectedException =
                new ApiException(
                    $"Amount of failed login attempts exceeds the allowed amount, please wait for {loginLimiterSettings.TimeOutPeriodInSeconds / 60} minutes before trying again",
                    429);
            Assert.Equal(tooManyLoginsException?.StatusCode, expectedException.StatusCode);
            Assert.Equal(tooManyLoginsException?.Message, expectedException.Message);
        }

        [Fact(DisplayName = "Login throws exception when email is not verified")]
        public async Task LoginFailsIfEmailIsNotVerified()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginFailsIfEmailIsNotVerified));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };

            var user = testuser;
            var somePass = "somePassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            // Login 
            var exception = (ApiException)Record.Exception(() => accountService.Login(user.Email, somePass, "2.1.0"));

            // Assert
            var expectedException = new ApiException("E-mail has not been verified", 403);
            Assert.Equal(exception.StatusCode, expectedException.StatusCode);
            Assert.Equal(exception.Message, expectedException.Message);
        }

        [Fact(DisplayName = "Login succeeds when email is verified")]
        public async Task LoginSucceedsIfEmailIsVerified()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginSucceedsIfEmailIsVerified));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };

            var user = testuser;
            user.IsVerified = true;

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            var hashService = new Mock<IHashService>();
            hashService.Setup(m => m.Hash(It.IsAny<string>())).Returns(user.Password);

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, hashService.Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            // Login 
            var result = accountService.Login(user.Email, user.Password, "2.1.0");

            // Assert we did not fail in the above call. This test does not test the result
            Assert.Null(result);
        }

        [Fact(DisplayName = "Login with unknown user throws API Exception")]
        public async Task LoginWithUnknownUserThrowsApiException()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginWithUnknownUserThrowsApiException));

            var databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };


            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            await context.SaveChangesAsync();

            // Act
            var accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            // Login 
            var exception = (ApiException)Record.Exception(() => accountService.Login("unknown email", "somePass", "2.1.0"));

            // Assert
            var expectedException = new ApiException("The username or password does not match", 401);
            Assert.Equal(exception.StatusCode, expectedException.StatusCode);
            Assert.Equal(exception.Message, expectedException.Message);
        }

        [Theory(DisplayName = "VerifyRegistration returns false on invalid token")]
        [MemberData(nameof(TokenGenerator))]
        public async Task VerifyRegistrationReturnsFalseOnInvalidToken(string token)
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(VerifyRegistrationReturnsFalseOnInvalidToken) +
                                     token);

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };
            var identitySettings = new IdentitySettings
            {
                TokenKey = "This is a long test token key"
            };
            var tokenService = new TokenService(identitySettings, new ClaimsUtilities(context));

            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);
            var accountService = new AccountService(context, environmentSettings, tokenService,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            //Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => accountService.VerifyRegistration(token));
        }

        [Fact(DisplayName = "VerifyRegistration returns true given valid token")]
        public async Task VerifyRegistrationReturnsTrueGivenValidToken()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(VerifyRegistrationReturnsTrueGivenValidToken));

            var databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            var environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using var context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var loginLimiterSettings = new LoginLimiterSettings
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };
            var identitySettings = new IdentitySettings
            {
                TokenKey = "This is a long test token key"
            };
            var tokenService = new TokenService(identitySettings, new ClaimsUtilities(context));

            httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);
            var accountService = new AccountService(context, environmentSettings, tokenService,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            var user = testuser;
            var token = WriteTokenString(new List<Claim>
            {
                new Claim(ClaimTypes.Role, "verification_token"),
                new Claim(ClaimTypes.Email, user.Email)
            });
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            //Act
            var result = await accountService.VerifyRegistration(token);

            //Assert
            Assert.True(result);
        }

        public static IEnumerable<object[]> TokenGenerator()
        {
            yield return new object[] {
                "Malformed token"
            };
            yield return new object[] {
                WriteTokenString(new List<Claim> // Incorrect role
                {
                    new Claim(ClaimTypes.Role, "")
                })
            };
            yield return new object[] {
                WriteTokenString(new List<Claim> // No email claim
                {
                    new Claim(ClaimTypes.Role, "verification_token")
                })
            };
            yield return new object[] {
                WriteTokenString(new List<Claim> // Good token, assuming user can be found in database
                {
                    new Claim(ClaimTypes.Role, "verification_token"),
                    new Claim(ClaimTypes.Email, "test@test.test")
                })
            };
        }

        private static string WriteTokenString(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("This is a long test token key"));

            var jwt = new JwtSecurityToken("AnalogIO",
                "Everyone",
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(24),
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return tokenHandler.WriteToken(jwt);
        }
    }
}
