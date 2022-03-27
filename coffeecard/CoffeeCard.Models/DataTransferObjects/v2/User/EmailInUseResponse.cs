using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Contains information about an email being in use
    /// </summary>
    /// <example>
    /// {
    ///     "InUse": true,
    /// }
    /// </example>
    public class EmailInUseResponse
    {
        /// <summary>
        /// Contains information about an email being in use
        /// </summary>
        [Required]
        public bool InUse { get; set; }
    }
}