using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// User information that is returned when searching
    /// </summary>
    /// <example>
    /// {
    ///     "id": 123,
    ///     "name": "John Doe",
    ///     "email": "john@doe.com",
    ///     "role": "Barista"
    /// }
    /// </example>
    public class UserSearchResponse
    {
        /// <summary>
        /// User Id
        /// </summary>
        /// <value>User Id</value>
        /// <example>123</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Full Name of user
        /// </summary>
        /// <value>Full Name</value>
        /// <example>John Doe</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Email of user
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// User's role
        /// </summary>
        /// <value>Role</value>
        /// <example>Barista</example>
        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;
    }
}