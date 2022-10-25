using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.V2.User
{
    /// <summary>
    /// Register a new user data object
    /// </summary>
    /// <example>
    /// {
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "password": "[no example provided]",
    ///     "programme": 1
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
        /// Pin Code as first UTF8 encoded, then SHA256 hashed, and then Base64 encoded string
        /// </summary>
        /// <value>Password</value>
        /// <example>[no example provided]</example>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Programme of user
        /// </summary>
        /// <value>Programme Id</value>
        /// <example>1</example>
        [Required]
        public int ProgrammeId { get; set; }
    }
}