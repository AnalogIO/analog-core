using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.User
{
    /// <summary>
    /// Register a new user data object
    /// </summary>
    /// <example>
    /// {
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "password": "[no example provided]"
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
        public required string Name { get; set; } = string.Empty;

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
        /// <value>Password</value>
        /// <example>[no example provided]</example>
        [Required]
        public required string Password { get; set; } = string.Empty;
    }
}
