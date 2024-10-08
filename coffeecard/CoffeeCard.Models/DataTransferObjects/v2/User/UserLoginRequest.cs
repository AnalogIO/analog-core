using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// User login request object
    /// </summary>
    /// <example>
    /// {
    ///     "email": "john@doe.com",
    /// }
    /// </example>
    public class UserLoginRequest
    {
        /// <summary>
        /// Email of user
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [EmailAddress]
        public string Email { get; set; } = null!;

        public LoginType LoginType { get; set; }
    }
}