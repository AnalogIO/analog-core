using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.User
{
    /// <summary>
    /// Login data object
    /// </summary>
    /// <example>
    /// {
    ///     "email": "john@doe.com",
    ///     "password": "[no example provided]",
    ///     "version": "2.1.0"
    /// }
    /// </example>
    public class LoginDto
    {
        /// <summary>
        /// Email Address of user
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        /// <summary>
        /// Pin Code as first UTF8 encoded, then SHA256 hashed, and then Base64 encoded string
        /// </summary>
        /// <value>Pin code</value>
        /// <example>[no example provided]</example>
        [Required]
        public required string Password { get; set; } = string.Empty;

        /// <summary>
        /// App version of device logging in
        /// </summary>
        /// <value>App version</value>
        /// <example>2.1.0</example>
        [Required]
        public required string Version { get; set; } = string.Empty;
    }
}
