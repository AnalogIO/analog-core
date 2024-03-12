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
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class AccountServiceTest
    {
        [Fact(DisplayName = "RecoverUser given malformed token returns false")]
        public async Task RecoverUserGivenMalformedTokenReturnsFalse()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenMalformedTokenReturnsFalse));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            bool expectedResult = false;

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);
            bool result = await accountService.RecoverUserAsync("bogus", "3433");

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token returns true")]
        public async Task RecoverUserGivenValidTokenReturnsTrue()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenValidTokenReturnsTrue));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            bool expectedResult = true;

            Claim claim = new Claim(ClaimTypes.Email, "test@email.dk");
            List<Claim> claims = [claim];
            JwtSecurityToken validToken = new JwtSecurityToken("analog", "all", claims);

            Mock<ITokenService> tokenService = new Mock<ITokenService>();
            _ = tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            _ = tokenService.Setup(t => t.ValidateTokenIsUnusedAsync("valid")).ReturnsAsync(true);

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            Token token = new Token("valid");
            List<Token> userTokens = [token];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };

            User user = new User { Tokens = userTokens, Programme = programme, Email = "test@email.dk", Name = "test", Password = "pass", Salt = "salt" };
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            AccountService accountService = new AccountService(context, environmentSettings, tokenService.Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);

            bool result = await accountService.RecoverUserAsync("valid", "3433");

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token updates password and resets users tokens")]
        public async Task RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            Claim claim = new Claim(ClaimTypes.Email, "test@email.dk");
            List<Claim> claims = [claim];
            JwtSecurityToken validToken = new JwtSecurityToken("analog", "all", claims);

            Mock<ITokenService> tokenService = new Mock<ITokenService>();
            _ = tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            _ = tokenService.Setup(t => t.ValidateTokenIsUnusedAsync("valid")).ReturnsAsync(true);

            string userPass = "not set";

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            Token token = new Token("valid");
            List<Token> userTokens = [token];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };

            User user = new User
            { Tokens = userTokens, Email = "test@email.dk", Name = "test", Programme = programme, Password = userPass, Salt = "salt" };
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            AccountService accountService = new AccountService(context, environmentSettings, tokenService.Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object, new Mock<ILoginLimiter>().Object, loginLimiterSettings);

            _ = await accountService.RecoverUserAsync("valid", "3433");

            User updatedUser = context.Users.FirstOrDefault(u => u.Email == user.Email);
            string newUserPass = updatedUser?.Password;
            ICollection<Token> newUserTokens = updatedUser?.Tokens;

            int expectedTokenCount = 0;

            // Assert
            Assert.NotEqual(newUserPass, userPass);
            Assert.Equal(newUserTokens?.Count, expectedTokenCount);
        }

        [Fact(DisplayName = "Login given valid credentials returns token")]
        public async Task LoginGivenValidCredentialsReturnsToken()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginGivenValidCredentialsReturnsToken));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            List<Token> userTokens = [];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };

            User user = new User
            {
                Id = 1,
                Name = "test",
                Tokens = userTokens,
                Email = "test@email.dk",
                Programme = programme,
                Password = "test",
                Salt = "salt",
                IsVerified = true
            };

            Mock<IHashService> hasher = new Mock<IHashService>();
            _ = hasher.Setup(h => h.Hash(user.Password + user.Salt)).Returns(user.Password);

            string expectedToken = "valid";

            Mock<ITokenService> tokenService = new Mock<ITokenService>();
            _ = tokenService.Setup(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>())).Returns(expectedToken);

            Mock<ILoginLimiter> loginLimiter = new Mock<ILoginLimiter>();
            _ = loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            AccountService accountService = new AccountService(context, environmentSettings, tokenService.Object,
                new Mock<IEmailService>().Object, hasher.Object,
                new Mock<IHttpContextAccessor>().Object, loginLimiter.Object, loginLimiterSettings);

            string actualToken = accountService.Login(user.Email, user.Password, "2.1.0");

            // Assert
            Assert.Equal(expectedToken, actualToken);
        }

        [Fact(DisplayName = "LoginLimiter is called if limiter is enabled")]
        public async Task LoginRejectsAfterFiveFailedLogins()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginRejectsAfterFiveFailedLogins));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            List<Token> userTokens = [];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };

            User user = new User
            {
                Id = 1,
                Name = "test",
                Tokens = userTokens,
                Email = "test@email.dk",
                Programme = programme,
                Password = "test",
                Salt = "salt",
                IsVerified = true
            };

            string wrongPass = "wrongPassword";

            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            Mock<ILoginLimiter> loginLimiter = new Mock<ILoginLimiter>();
            _ = loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                loginLimiter.Object, loginLimiterSettings);

            //Attempts to login
            _ = Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            // Assert
            loginLimiter.Verify(l => l.LoginAllowed(user), Times.Once);
        }

        [Fact(DisplayName = "LoginLimiter not called if limiter is disabled")]
        public async Task LoginLimiterNotCalledWhenDisabled()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginLimiterNotCalledWhenDisabled));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = false,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 5
            };

            List<Token> userTokens = [];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };

            User user = new User
            {
                Id = 1,
                Name = "test",
                Tokens = userTokens,
                Email = "test@email.dk",
                Programme = programme,
                Password = "test",
                Salt = "salt",
                IsVerified = true
            };

            string wrongPass = "wrongPassword";

            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            Mock<ILoginLimiter> loginLimiter = new Mock<ILoginLimiter>();
            _ = loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                loginLimiter.Object, loginLimiterSettings);

            //Attempts to login
            _ = Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            // Assert
            loginLimiter.Verify(l => l.LoginAllowed(user), Times.Never);
        }

        [Fact(DisplayName = "Login throws exception when limit is reached and limiter is enabled")]
        public async Task LoginThrowsExceptionWhenLimitIsReached()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginThrowsExceptionWhenLimitIsReached));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };

            List<Token> userTokens = [];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };
            User user = new User
            {
                Id = 1,
                Name = "test",
                Tokens = userTokens,
                Email = "test@email.dk",
                Programme = programme,
                Password = "test",
                Salt = "salt",
                IsVerified = true
            };

            string wrongPass = "wrongPassword";

            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            // Act
            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            //Attempts to login with the wrong credentials 
            _ = Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            //Attempts to login a sixth time with the same credentials and captures the exception 
            ApiException tooManyLoginsException =
                (ApiException)Record.Exception(() => accountService.Login(user.Email, wrongPass, "2.1.0"));
            // Assert
            ApiException expectedException =
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
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginFailsIfEmailIsNotVerified));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };

            List<Token> userTokens = [];
            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };
            User user = new User
            {
                Id = 1,
                Name = "test",
                Tokens = userTokens,
                Email = "test@email.dk",
                Programme = programme,
                Password = "test",
                Salt = "salt",
                IsVerified = false
            };
            string somePass = "somePassword";

            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            // Act
            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new HashService(), httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            // Login 
            ApiException exception = (ApiException)Record.Exception(() => accountService.Login(user.Email, somePass, "2.1.0"));

            // Assert
            ApiException expectedException = new ApiException("E-mail has not been verified", 403);
            Assert.Equal(exception.StatusCode, expectedException.StatusCode);
            Assert.Equal(exception.Message, expectedException.Message);
        }

        [Fact(DisplayName = "Login succeeds when email is verified")]
        public async Task LoginSucceedsIfEmailIsVerified()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginSucceedsIfEmailIsVerified));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };

            List<Token> userTokens = [];
            string somePass = "somePassword";
            Programme programme = new Programme() { FullName = "fullName", ShortName = "shortName" };
            User user = new User
            {
                Id = 1,
                Name = "test",
                Tokens = userTokens,
                Email = "test@email.dk",
                Programme = programme,
                Password = somePass,
                Salt = "salt",
                IsVerified = true
            };
            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            Mock<IHashService> hashService = new Mock<IHashService>();
            _ = hashService.Setup(m => m.Hash(It.IsAny<string>())).Returns(somePass);

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.AddAsync(user);
            _ = await context.SaveChangesAsync();

            // Act
            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, hashService.Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            // Login 
            string result = accountService.Login(user.Email, somePass, "2.1.0");

            // Assert we did not fail in the above call. This test does not test the result
            Assert.Null(result);
        }

        [Fact(DisplayName = "Login with unknown user throws API Exception")]
        public async Task LoginWithUnknownUserThrowsApiException()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(LoginWithUnknownUserThrowsApiException));

            DatabaseSettings databaseSettings = new DatabaseSettings()
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            { DeploymentUrl = "test", EnvironmentType = EnvironmentType.Test, MinAppVersion = "2.1.0" };
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };


            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);
            _ = await context.SaveChangesAsync();

            // Act
            AccountService accountService = new AccountService(context, environmentSettings, new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            // Login 
            ApiException exception = (ApiException)Record.Exception(() => accountService.Login("unknown email", "somePass", "2.1.0"));

            // Assert
            ApiException expectedException = new ApiException("The username or password does not match", 401);
            Assert.Equal(exception.StatusCode, expectedException.StatusCode);
            Assert.Equal(exception.Message, expectedException.Message);
        }

        [Theory(DisplayName = "VerifyRegistration returns false on invalid token")]
        [MemberData(nameof(TokenGenerator))]
        public async Task VerifyRegistrationReturnsFalseOnInvalidToken(string token)
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(VerifyRegistrationReturnsFalseOnInvalidToken) +
                                     token);

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };
            IdentitySettings identitySettings = new IdentitySettings
            {
                TokenKey = "This is a long test token key"
            };
            TokenService tokenService = new TokenService(identitySettings, new ClaimsUtilities(context));

            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);
            AccountService accountService = new AccountService(context, environmentSettings, tokenService,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            //Act & Assert
            _ = await Assert.ThrowsAsync<ApiException>(() => accountService.VerifyRegistration(token));
        }

        [Fact(DisplayName = "VerifyRegistration returns true given valid token")]
        public async Task VerifyRegistrationReturnsTrueGivenValidToken()
        {
            // Arrange
            DbContextOptionsBuilder<CoffeeCardContext> builder = new DbContextOptionsBuilder<CoffeeCardContext>()
                .UseInMemoryDatabase(nameof(VerifyRegistrationReturnsTrueGivenValidToken));

            DatabaseSettings databaseSettings = new DatabaseSettings
            {
                SchemaName = "test"
            };
            EnvironmentSettings environmentSettings = new EnvironmentSettings()
            {
                EnvironmentType = EnvironmentType.Test
            };

            await using CoffeeCardContext context = new CoffeeCardContext(builder.Options, databaseSettings, environmentSettings);

            Mock<IHttpContextAccessor> httpContextAccessor = new Mock<IHttpContextAccessor>();
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 1,
                TimeOutPeriodInSeconds = 1
            };
            IdentitySettings identitySettings = new IdentitySettings
            {
                TokenKey = "SuperLongSigningKeySuperLongSigningKey"
            };
            TokenService tokenService = new TokenService(identitySettings, new ClaimsUtilities(context));

            _ = httpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext().HttpContext);
            AccountService accountService = new AccountService(context, environmentSettings, tokenService,
                new Mock<IEmailService>().Object, new Mock<IHashService>().Object, httpContextAccessor.Object,
                new LoginLimiter(loginLimiterSettings), loginLimiterSettings);

            string token = WriteTokenString(
            [
                new Claim(ClaimTypes.Role, "verification_token"),
                new Claim(ClaimTypes.Email, "test@test.test")
            ]);

            Programme programme = new Programme { FullName = "fullName", ShortName = "shortName" };
            User user = new User
            {
                Email = "test@test.test",
                Name = "test",
                Password = "pass",
                Salt = "salt",
                Programme = programme

            };
            _ = await context.Users.AddAsync(user);
            _ = await context.SaveChangesAsync();

            //Act
            bool result = await accountService.VerifyRegistration(token);

            //Assert
            Assert.True(result);
        }

        public static IEnumerable<object[]> TokenGenerator()
        {
            yield return new object[] {
                "Malformed token"
            };
            yield return new object[] {
                WriteTokenString(
                // Incorrect role
                [
                    new Claim(ClaimTypes.Role, "")
                ])
            };
            yield return new object[] {
                WriteTokenString(
                // No email claim
                [
                    new Claim(ClaimTypes.Role, "verification_token")
                ])
            };
            yield return new object[] {
                WriteTokenString(
                // Good token, assuming user can be found in database
                [
                    new Claim(ClaimTypes.Role, "verification_token"),
                    new Claim(ClaimTypes.Email, "test@test.test")
                ])
            };
        }

        private static string WriteTokenString(IEnumerable<Claim> claims)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SuperLongSigningKeySuperLongSigningKey"));

            JwtSecurityToken jwt = new JwtSecurityToken("AnalogIO",
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
