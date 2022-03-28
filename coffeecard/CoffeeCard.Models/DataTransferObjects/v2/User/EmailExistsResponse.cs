using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Contains information about an email being in use
    /// </summary>
    /// <example>
    /// {
    ///     "EmailExists": true
    /// }
    /// </example>
    public class EmailExistsResponse
    {
        /// <summary>
        /// Contains information about an email being in use
        /// </summary>
        [Required]
        public bool EmailExists { get; set; }
    }
}