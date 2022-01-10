using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.User
{
    /// <summary>
    /// Login data object
    /// </summary>
    /// <example>
    /// {
    ///     "email": "john@doe.com",
    ///     "password": "0ffe1abd1a08215353c233d6e009613e95eec4253832a761af28ff37ac5a150c",
    ///     "version": "2.0.0"
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
        public string Email { get; set; }

        /// <summary>
        /// Pin Code as first SHA256, then Base64 encoded string
        /// </summary>
        /// <value>Pin code</value>
        /// <example>no example provided</example>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// App version of device logging in
        /// </summary>
        /// <value>App version</value>
        /// <example>2.0.0</example>
        [Required]
        public string Version { get; set; }
    }
}