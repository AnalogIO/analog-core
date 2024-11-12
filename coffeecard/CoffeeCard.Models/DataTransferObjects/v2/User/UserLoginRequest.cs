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

        /// <summary>
        /// Defines which application should open on login
        /// </summary>
        /// <value>LoginType</value>
        /// <example>Shifty</example>
        public LoginType LoginType { get; set; }
    }
}