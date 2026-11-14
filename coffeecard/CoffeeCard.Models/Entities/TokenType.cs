using System;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Enum for the different types of token
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Token used to reset a user's password
        /// </summary>
        ResetPassword,

        /// <summary>
        /// Token used to delete a user's account
        /// </summary>
        DeleteAccount,

        /// <summary>
        /// Token used to log in to an application
        /// </summary>
        MagicLink,

        /// <summary>
        /// Token used to refresh a JWT token
        /// </summary>
        Refresh,
    }

    /// <summary>
    /// Extension methods for TokenType
    /// </summary>
    public static class TokenTypeExtensions
    {
        /// <summary>
        /// Get the default date and time when a token expires
        /// </summary>
        public static DateTime getExpiresAt(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ResetPassword => DateTime.UtcNow.AddDays(1),
                TokenType.DeleteAccount => DateTime.UtcNow.AddDays(1),
                TokenType.MagicLink => DateTime.UtcNow.AddMinutes(30),
                TokenType.Refresh => DateTime.UtcNow.AddMonths(1),
                _ => DateTime.UtcNow.AddDays(1),
            };
        }
    }
}
