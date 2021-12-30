using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.User
{
    /// <summary>
    /// User email data object
    /// </summary>
    /// <example>
    /// {
    ///    "email": "john@doe.com"
    /// }
    /// </example>
    public class EmailDto
    {
        /// <summary>
        /// User Email
        /// </summary>
        /// <value>Email</value>
        /// <example>john@doe.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}