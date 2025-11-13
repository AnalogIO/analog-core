using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.Entities;
using CoffeeCard.Tests.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class AccountServiceTest : BaseUnitTests
    {
        private readonly EnvironmentSettings _environmentSettings = new()
        {
            DeploymentUrl = "test",
            EnvironmentType = EnvironmentType.Test,
            MinAppVersion = "2.1.0",
        };
        private readonly LoginLimiterSettings _loginLimiterSettings = new()
        {
            IsEnabled = true,
            MaximumLoginAttemptsWithinTimeOut = 5,
            TimeOutPeriodInSeconds = 5,
        };

        [Fact(DisplayName = "RecoverUser given malformed token returns false")]
        public async Task RecoverUserGivenMalformedTokenReturnsFalse()
        {
            // Arrange
            var expectedResult = false;

            // Act
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<ILoginLimiter>().Object,
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );
            var result = await accountService.RecoverUserAsync("bogus", "3433");

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "RecoverUser given valid token returns true")]
        public async Task RecoverUserGivenValidTokenReturnsTrue()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.Email, "test@email.dk");
            var claims = new List<Claim> { claim };
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateTokenIsUnusedAsync("valid")).ReturnsAsync(true);

            // Act
            var token = new Token("valid", TokenType.ResetPassword);
            var userTokens = new List<Token> { token };
            var programme = new Programme { FullName = "fullName", ShortName = "shortName" };

            var user = new User
            {
                Tokens = userTokens,
                Programme = programme,
                Email = "test@email.dk",
                Name = "test",
                Password = "pass",
                Salt = "salt",
            };
            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                tokenService.Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<ILoginLimiter>().Object,
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            var result = await accountService.RecoverUserAsync("valid", "3433");

            var expectedResult = true;
            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(
            Skip = "Temporarily disabled until decision is made whether tokens should be reset, currently that behaviour does not exist",
            DisplayName = "RecoverUser given valid token updates password and resets users tokens"
        )]
        public async Task RecoverUserGivenValidTokenUpdatesPasswordAndResetsUsersTokens()
        {
            // Arrange
            var token = new Token("valid", TokenType.ResetPassword);

            var userPass = "not set";
            var user = UserBuilder
                .DefaultCustomer()
                .WithTokens([token])
                .WithPassword(userPass)
                .Build();

            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            var claim = new Claim(ClaimTypes.Email, user.Email);
            var claims = new List<Claim> { claim };
            var validToken = new JwtSecurityToken("analog", "all", claims);

            var tokenService = new Mock<ITokenService>();
            tokenService.Setup(t => t.ReadToken("valid")).Returns(validToken);
            tokenService.Setup(t => t.ValidateTokenIsUnusedAsync("valid")).ReturnsAsync(true);

            // Act
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                tokenService.Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<ILoginLimiter>().Object,
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            await accountService.RecoverUserAsync("valid", "3433");

            var updatedUser = AssertionContext
                .Users.Include(u => u.Tokens)
                .FirstOrDefault(u => u.Email == user.Email);
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
            var user = UserBuilder.DefaultCustomer().Build();

            var hasher = new Mock<IHashService>();
            hasher.Setup(h => h.Hash(user.Password + user.Salt)).Returns(user.Password);

            var expectedToken = "valid";

            var tokenService = new Mock<ITokenService>();
            tokenService
                .Setup(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns(expectedToken);

            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(It.IsAny<User>())).Returns(true);

            // Act
            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                tokenService.Object,
                new Mock<IEmailService>().Object,
                hasher.Object,
                new Mock<IHttpContextAccessor>().Object,
                loginLimiter.Object,
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            var actualToken = accountService.Login(user.Email, user.Password, "2.1.0");

            // Assert
            Assert.Equal(expectedToken, actualToken);
        }

        [Fact(DisplayName = "LoginLimiter is called if limiter is enabled")]
        public async Task LoginRejectsAfterFiveFailedLogins()
        {
            var user = UserBuilder.DefaultCustomer().Build();

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);

            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(It.IsAny<User>())).Returns(false);

            // Act
            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new HashService(),
                httpContextAccessor.Object,
                loginLimiter.Object,
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            //Attempts to login
            Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            // Assert
            loginLimiter.Verify(l => l.LoginAllowed(It.IsAny<User>()), Times.Once);
        }

        [Fact(DisplayName = "LoginLimiter not called if limiter is disabled")]
        public async Task LoginLimiterNotCalledWhenDisabled()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);

            var loginLimiter = new Mock<ILoginLimiter>();
            loginLimiter.Setup(l => l.LoginAllowed(user)).Returns(true);

            // Act
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new HashService(),
                httpContextAccessor.Object,
                loginLimiter.Object,
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            //Attempts to login
            Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            // Assert
            loginLimiter.Verify(l => l.LoginAllowed(user), Times.Never);
        }

        [Fact(DisplayName = "Login throws exception when limit is reached and limiter is enabled")]
        public async Task LoginThrowsExceptionWhenLimitIsReached()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            var wrongPass = "wrongPassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);

            // Act
            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            _loginLimiterSettings.MaximumLoginAttemptsWithinTimeOut = 1;
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new HashService(),
                httpContextAccessor.Object,
                new LoginLimiter(_loginLimiterSettings, NullLogger<LoginLimiter>.Instance),
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            //Attempts to login with the wrong credentials
            Assert.Throws<ApiException>(() => accountService.Login(user.Email, wrongPass, "2.1.0"));

            //Attempts to login a sixth time with the same credentials and captures the exception
            var tooManyLoginsException = (ApiException)
                Record.Exception(() => accountService.Login(user.Email, wrongPass, "2.1.0"));
            // Assert
            var expectedException = new ApiException(
                $"Amount of failed login attempts exceeds the allowed amount, please wait for {_loginLimiterSettings.TimeOutPeriodInSeconds / 60} minutes before trying again",
                429
            );
            Assert.Equal(expectedException.StatusCode, tooManyLoginsException?.StatusCode);
            Assert.Equal(expectedException.Message, tooManyLoginsException?.Message);
        }

        [Fact(DisplayName = "Login throws exception when email is not verified")]
        public async Task LoginFailsIfEmailIsNotVerified()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().WithIsVerified(false).Build();
            var somePass = "somePassword";

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);

            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            // Act
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new HashService(),
                httpContextAccessor.Object,
                new LoginLimiter(_loginLimiterSettings, NullLogger<LoginLimiter>.Instance),
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            // Login
            var exception = (ApiException)
                Record.Exception(() => accountService.Login(user.Email, somePass, "2.1.0"));

            // Assert
            var expectedException = new ApiException("E-mail has not been verified", 403);
            Assert.Equal(exception.StatusCode, expectedException.StatusCode);
            Assert.Equal(exception.Message, expectedException.Message);
        }

        [Fact(DisplayName = "Login succeeds when email is verified")]
        public async Task LoginSucceedsIfEmailIsVerified()
        {
            // Arrange
            var user = UserBuilder.DefaultCustomer().Build();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);

            var hashService = new Mock<IHashService>();
            hashService.Setup(m => m.Hash(It.IsAny<string>())).Returns(user.Password);

            await InitialContext.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            // Act
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                hashService.Object,
                httpContextAccessor.Object,
                new LoginLimiter(_loginLimiterSettings, NullLogger<LoginLimiter>.Instance),
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            // Login
            var result = accountService.Login(user.Email, user.Password, "2.1.0");

            // Assert we did not fail in the above call. This test does not test the result
            Assert.Null(result);
        }

        [Fact(DisplayName = "Login with unknown user throws API Exception")]
        public async Task LoginWithUnknownUserThrowsApiException()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);

            // Act
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                new Mock<ITokenService>().Object,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object,
                httpContextAccessor.Object,
                new LoginLimiter(_loginLimiterSettings, NullLogger<LoginLimiter>.Instance),
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            // Login
            var exception = (ApiException)
                Record.Exception(() => accountService.Login("unknown email", "somePass", "2.1.0"));

            // Assert
            var expectedException = new ApiException(
                "The username or password does not match",
                401
            );
            Assert.Equal(exception.StatusCode, expectedException.StatusCode);
            Assert.Equal(exception.Message, expectedException.Message);
        }

        [Theory(DisplayName = "VerifyRegistration returns false on invalid token")]
        [MemberData(nameof(TokenGenerator))]
        public async Task VerifyRegistrationReturnsFalseOnInvalidToken(string token)
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var identitySettings = new IdentitySettings
            {
                TokenKey = "This is a long test token key",
            };
            var tokenService = new TokenService(
                identitySettings,
                new ClaimsUtilities(AssertionContext),
                NullLogger<TokenService>.Instance
            );

            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                tokenService,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object,
                httpContextAccessor.Object,
                new LoginLimiter(_loginLimiterSettings, NullLogger<LoginLimiter>.Instance),
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            //Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => accountService.VerifyRegistration(token));
        }

        [Fact(DisplayName = "VerifyRegistration returns true given valid token")]
        public async Task VerifyRegistrationReturnsTrueGivenValidToken()
        {
            // Arrange
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var identitySettings = new IdentitySettings
            {
                TokenKey = "SuperLongSigningKeySuperLongSigningKey",
            };
            var tokenService = new TokenService(
                identitySettings,
                new ClaimsUtilities(AssertionContext),
                NullLogger<TokenService>.Instance
            );

            httpContextAccessor
                .Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext().HttpContext);
            var accountService = new AccountService(
                AssertionContext,
                _environmentSettings,
                tokenService,
                new Mock<IEmailService>().Object,
                new Mock<IHashService>().Object,
                httpContextAccessor.Object,
                new LoginLimiter(_loginLimiterSettings, NullLogger<LoginLimiter>.Instance),
                _loginLimiterSettings,
                NullLogger<AccountService>.Instance
            );

            var user = UserBuilder.DefaultCustomer().WithIsVerified(false).Build();
            var token = WriteTokenString(
                new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "verification_token"),
                    new Claim(ClaimTypes.Email, user.Email),
                }
            );

            await InitialContext.Users.AddAsync(user);
            await InitialContext.SaveChangesAsync();

            //Act
            var result = await accountService.VerifyRegistration(token);

            //Assert
            Assert.True(result);
        }

        public static IEnumerable<object[]> TokenGenerator()
        {
            yield return new object[] { "Malformed token" };
            yield return new object[]
            {
                WriteTokenString(
                    new List<Claim> // Incorrect role
                    {
                        new Claim(ClaimTypes.Role, ""),
                    }
                ),
            };
            yield return new object[]
            {
                WriteTokenString(
                    new List<Claim> // No email claim
                    {
                        new Claim(ClaimTypes.Role, "verification_token"),
                    }
                ),
            };
            yield return new object[]
            {
                WriteTokenString(
                    new List<Claim> // Good token, assuming user can be found in database
                    {
                        new Claim(ClaimTypes.Role, "verification_token"),
                        new Claim(ClaimTypes.Email, "test@test.test"),
                    }
                ),
            };
        }

        private static string WriteTokenString(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SuperLongSigningKeySuperLongSigningKey")
            );

            var jwt = new JwtSecurityToken(
                "AnalogIO",
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
