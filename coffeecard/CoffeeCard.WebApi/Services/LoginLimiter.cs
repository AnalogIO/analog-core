using System;
using System.Collections;
using System.Collections.Concurrent;
using CoffeeCard.Common.Configuration;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Cms;
using Serilog;

namespace CoffeeCard.WebApi.Services
{
    public class LoginLimiter : ILoginLimiter
    {
        private ConcurrentDictionary<string, (DateTime, int)> _loginAttempts;
        private readonly IdentitySettings _identitySettings;

        public LoginLimiter(IdentitySettings identitySettings)
        {
            _loginAttempts = new ConcurrentDictionary<string, (DateTime, int)>();
            _identitySettings = identitySettings;
        }

        /// <summary>
        /// Receives an email address to update the count for.
        /// If no count currently exist for said email, creates a new one count with the current time.
        /// If a count exist, increment the counter by 1 for said email, and preserve the time.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>
        /// The value that the key was updated with
        /// </returns>
        private (DateTime, int) UpdateAndGetLoginAttemptCount(string email)
        {
            return _loginAttempts.AddOrUpdate(email, (DateTime.UtcNow, 0), (key, value) => (value.Item1, value.Item2 +1));
        }

        /// <summary>
        /// Resets the first failed login time and in so doing, extends the lockout period
        /// </summary>
        /// <param name="email"></param>
        private void ResetUsersTimeoutPeriod(string email)
        {
            if (!_loginAttempts.TryGetValue(email, out var oldEntry)) return;
            if (_loginAttempts.TryUpdate(email, (DateTime.UtcNow, oldEntry.Item2), oldEntry))
                Log.Warning("The lockout period for {username} was reset, possible brute force attack", email);
        }

        /// <summary>
        /// Determines if the given user has too many failed logins within the timeout period
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool LoginAllowed(User user)
        {
            var (firstFailedLogin, loginAttemptsMade) = UpdateAndGetLoginAttemptCount(user.Email);
            var timeOutPeriod = new TimeSpan(0, _identitySettings.TimeOutPeriodInMinutes, 0);
            var timeSinceFirstFailedLogin = DateTime.UtcNow.Subtract(firstFailedLogin);

            
            if(loginAttemptsMade % 5 == 0 && timeSinceFirstFailedLogin > timeOutPeriod) ResetUsersTimeoutPeriod(user.Email);

            return loginAttemptsMade < _identitySettings.MaximumLoginAttemptsWithinTimeOut || timeSinceFirstFailedLogin > timeOutPeriod;
        }

        /// <summary>
        /// Removes the given email from the loginAttempts dictionary, thereby resetting the attempts made
        /// </summary>
        /// <param name="user"></param>
        public void ResetLoginAttemptsForUser(User user)
        {
            _loginAttempts.TryRemove(user.Email, out var value);
        }
    }
}