using System;
using System.Collections.Concurrent;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Models.Entities;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services
{
    public class LoginLimiter : ILoginLimiter
    {
        private readonly ConcurrentDictionary<string, (DateTime, int)> _loginAttempts;
        private readonly LoginLimiterSettings _loginLimiterSettings;
        private readonly ILogger<LoginLimiter> _logger;

        public LoginLimiter(LoginLimiterSettings loginLimiterSettings, ILogger<LoginLimiter> logger)
        {
            _loginAttempts = new ConcurrentDictionary<string, (DateTime, int)>();
            _loginLimiterSettings = loginLimiterSettings;
            _logger = logger;
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
            return _loginAttempts.AddOrUpdate(
                email,
                (DateTime.UtcNow, 0),
                (_, value) => (value.Item1, value.Item2 + 1)
            );
        }

        /// <summary>
        /// Resets the first failed login time and in so doing, extends the lockout period
        /// </summary>
        /// <param name="email"></param>
        private void ResetUsersTimeoutPeriod(string email)
        {
            if (!_loginAttempts.TryGetValue(email, out var oldEntry))
                return;
            if (_loginAttempts.TryUpdate(email, (DateTime.UtcNow, oldEntry.Item2), oldEntry))
                _logger.LogWarning(
                    "The lockout period for {username} was reset, possible brute force attack",
                    email
                );
        }

        /// <summary>
        /// Determines if the given user has too many failed logins within the timeout period
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool LoginAllowed(User user)
        {
            var (firstFailedLogin, loginAttemptsMade) = UpdateAndGetLoginAttemptCount(user.Email);
            var timeOutPeriod = new TimeSpan(0, 0, _loginLimiterSettings.TimeOutPeriodInSeconds);
            var timeSinceFirstFailedLogin = DateTime.UtcNow.Subtract(firstFailedLogin);

            var maximumLoginAttemptsWithinTimeOut =
                _loginLimiterSettings.MaximumLoginAttemptsWithinTimeOut;

            if (
                loginAttemptsMade % maximumLoginAttemptsWithinTimeOut == 0
                && timeSinceFirstFailedLogin > timeOutPeriod
            ) //If the timeout period is exceeded it will reset it whenever loginAttemptsMade % x = 0, where x is the maximum login attempts made. I.e. x number of logins will be allowed before the timer is reset again
                ResetUsersTimeoutPeriod(user.Email);

            return loginAttemptsMade < maximumLoginAttemptsWithinTimeOut
                || //If the amount of login attempts made exceeds the allowed amount, it returns false
                timeSinceFirstFailedLogin > timeOutPeriod; //If the time since first failed login exceeds the timeout period, returns true. This also means if the timeSinceFirstFailedLogin is never reset, all login attempts made after the initial timeout period will be allowed
        }

        /// <summary>
        /// Removes the given email from the loginAttempts dictionary, thereby resetting the attempts made
        /// </summary>
        /// <param name="user"></param>
        public void ResetLoginAttemptsForUser(User user)
        {
            _loginAttempts.TryRemove(user.Email, out _);
        }
    }
}
