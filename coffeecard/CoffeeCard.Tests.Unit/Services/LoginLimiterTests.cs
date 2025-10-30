using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Services;
using CoffeeCard.Tests.Common.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class LoginLimiterTests
    {
        [Fact(DisplayName = "LoginLimiter allows logins after timeout expired")]
        public async Task LoginAllowsLoginsAfterTimeout()
        {
            // Arrange
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 1,
            };
            var user = UserBuilder.DefaultCustomer().Build();

            var loginLimiter = new LoginLimiter(
                loginLimiterSettings,
                NullLogger<LoginLimiter>.Instance
            );

            const bool lockedOutExpected = false;
            const bool loginAllowedAgainExpected = true;

            // Act
            //Triggers the lockout by attempting login 5 times in a row
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);

            var lockedOutActual = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the initial attempts
            await Task.Delay(1100);
            var loginAllowedAgainActual = loginLimiter.LoginAllowed(user); //Checks that you are allowed to login again after the timeout period

            Assert.Equal(lockedOutExpected, lockedOutActual);
            Assert.Equal(loginAllowedAgainExpected, loginAllowedAgainActual);
        }

        [Fact(DisplayName = "Login can lockout twice")]
        public async Task LoginLockoutsTwice()
        {
            // Arrange
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 1,
            };
            var user = UserBuilder.DefaultCustomer().Build();

            var loginLimiter = new LoginLimiter(
                loginLimiterSettings,
                NullLogger<LoginLimiter>.Instance
            );

            // Act
            var allowedLoginResults = new List<bool>();
            //Triggers the lockout by attempting login 5 times in a row
            for (var i = 0; i < loginLimiterSettings.MaximumLoginAttemptsWithinTimeOut; i++)
            {
                allowedLoginResults.Add(loginLimiter.LoginAllowed(user));
            }
            var actualFirstLockoutResult = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the first series of attempts
            await Task.Delay(1100); //Passes the lockout time

            //Triggers the lockout again by another 5 attempted logins in a row
            for (var i = 0; i < loginLimiterSettings.MaximumLoginAttemptsWithinTimeOut; i++)
            {
                allowedLoginResults.Add(loginLimiter.LoginAllowed(user));
            }

            var actualLogoutResult = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the second series of attempts

            Assert.All(allowedLoginResults, Assert.True);
            Assert.False(actualFirstLockoutResult);
            Assert.False(actualLogoutResult);
        }
    }
}
