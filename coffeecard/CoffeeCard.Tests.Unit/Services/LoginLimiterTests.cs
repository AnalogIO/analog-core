using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class LoginLimiterTests
    {

        [Fact(DisplayName = "LoginLimiter allows logins after timeout expired")]
        public async Task LoginAllowsLoginsAfterTimeout()
        {
            // Arrange
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 1
            };
            User user = new User
            {
                Id = 1,
                Name = "test",
                Email = "test@email.dk",
                Programme = new Programme(),
                Password = "test",
                IsVerified = true
            };

            LoginLimiter loginLimiter = new LoginLimiter(loginLimiterSettings);

            const bool lockedOutExpected = false;
            const bool loginAllowedAgainExpected = true;

            // Act
            //Triggers the lockout by attempting login 5 times in a row
            _ = loginLimiter.LoginAllowed(user);
            _ = loginLimiter.LoginAllowed(user);
            _ = loginLimiter.LoginAllowed(user);
            _ = loginLimiter.LoginAllowed(user);
            _ = loginLimiter.LoginAllowed(user);

            bool lockedOutActual = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the initial attempts
            await Task.Delay(1100);
            bool loginAllowedAgainActual = loginLimiter.LoginAllowed(user); //Checks that you are allowed to login again after the timeout period

            Assert.Equal(lockedOutExpected, lockedOutActual);
            Assert.Equal(loginAllowedAgainExpected, loginAllowedAgainActual);
        }

        [Fact(DisplayName = "Login can lockout twice")]
        public async Task LoginLockoutsTwice()
        {
            // Arrange
            LoginLimiterSettings loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true,
                MaximumLoginAttemptsWithinTimeOut = 5,
                TimeOutPeriodInSeconds = 1
            };
            User user = new User
            {
                Id = 1,
                Name = "test",
                Email = "test@email.dk",
                Programme = new Programme(),
                Password = "test",
                IsVerified = true
            };

            LoginLimiter loginLimiter = new LoginLimiter(loginLimiterSettings);

            // Act
            List<bool> allowedLoginResults = [];
            //Triggers the lockout by attempting login 5 times in a row
            for (int i = 0; i < loginLimiterSettings.MaximumLoginAttemptsWithinTimeOut; i++)
            {
                allowedLoginResults.Add(loginLimiter.LoginAllowed(user));
            }
            bool actualFirstLockoutResult = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the first series of attempts
            await Task.Delay(1100); //Passes the lockout time


            //Triggers the lockout again by another 5 attempted logins in a row
            for (int i = 0; i < loginLimiterSettings.MaximumLoginAttemptsWithinTimeOut; i++)
            {
                allowedLoginResults.Add(loginLimiter.LoginAllowed(user));
            }

            bool actualLogoutResult = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the second series of attempts

            Assert.All(allowedLoginResults, Assert.True);
            Assert.False(actualFirstLockoutResult);
            Assert.False(actualLogoutResult);
        }
    }
}