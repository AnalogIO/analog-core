using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// An email that should be checked if it already exists
    /// </summary>
    /// <example>
    /// {
    ///     "email": "johndoe@mail.com"
    /// }
    /// </example>
    public class EmailInUseRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        /// <value>Email</value>
        /// <example>johndoe@mail.com</example>
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}