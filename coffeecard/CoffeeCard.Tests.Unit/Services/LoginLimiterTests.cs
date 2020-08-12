using System.Threading;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class LoginLimiterTests
    {
        
        [Fact(DisplayName = "LoginLimiter allows logins after timeout expired")]
        public void LoginAllowsLoginsAfterTimeout()
        {
            // Arrange
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 1
            };
            var user = new User
            {
                Id = 1, Name = "test", Email = "test@email.dk", Programme = new Programme(),
                Password = "test", IsVerified = true
            };
            
            var loginLimiter = new LoginLimiter(loginLimiterSettings);

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
            Thread.Sleep(61000);
            var loginAllowedAgainActual = loginLimiter.LoginAllowed(user); //Checks that you are allowed to login again after the timeout period
            
            Assert.Equal(lockedOutExpected, lockedOutActual);
            Assert.Equal(loginAllowedAgainExpected, loginAllowedAgainActual);
        }
        
        [Fact(DisplayName = "Login can lockout twice")]
        public void LoginLockoutsTwice()
        {
            // Arrange
            var loginLimiterSettings = new LoginLimiterSettings()
            {
                IsEnabled = true, MaximumLoginAttemptsWithinTimeOut = 5, TimeOutPeriodInMinutes = 1
            };
            var user = new User
            {
                Id = 1, Name = "test", Email = "test@email.dk", Programme = new Programme(),
                Password = "test", IsVerified = true
            };
            
            var loginLimiter = new LoginLimiter(loginLimiterSettings);
            
            // Act
            //Triggers the lockout by attempting login 5 times in a row
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            var actualFirstLockoutResult = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the first series of attempts
            Thread.Sleep(60001); //Passes the lockout time 

            //Triggers the lockout again by another 5 attempted logins in a row
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            loginLimiter.LoginAllowed(user);
            
            var actualLogoutResult = loginLimiter.LoginAllowed(user); //Checks that you are actually locked after the second series of attempts
            
            Assert.False(actualFirstLockoutResult);
            Assert.False(actualLogoutResult);
        }
    }
}