using System;
using System.Collections;
using System.Collections.Concurrent;

namespace CoffeeCard.WebApi.Services
{
    public class LoginLimiter : ILoginLimiter
    {
        private ConcurrentDictionary<string, (DateTime, int)> _loginAttempts;

        public LoginLimiter()
        {
            _loginAttempts = new ConcurrentDictionary<string, (DateTime, int)>();
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
        public (DateTime, int) UpdateAndGetLoginAttemptCount(string email)
        {
            return _loginAttempts.AddOrUpdate(email, (DateTime.UtcNow, 0), (key, value) => (value.Item1, value.Item2 +1));
        }

        /// <summary>
        /// Removes the given email from the loginAttempts dictionary
        /// </summary>
        /// <param name="email"></param>
        public void RemoveEntry(string email)
        {
            _loginAttempts.TryRemove(email, out var value);
        }
    }
}