using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.User
{
    /// <summary>
    /// Register a new user data object
    /// </summary>
    /// <example>
    /// {
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "password": "0ffe1abd1a08215353c233d6e009613e95eec4253832a761af28ff37ac5a150c"
    /// }
    /// </example>
    public class RegisterDto
    {
        /// <summary>
        /// Full Name of user
        /// </summary>
        /// <value>Full Name</value>
        /// <example>John Doe</example>
        [Required]
        public string Name { get; set; }

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
        /// <value>Password</value>
        /// <example>no example provided</example>
        [Required]
        public string Password { get; set; }
    }
}