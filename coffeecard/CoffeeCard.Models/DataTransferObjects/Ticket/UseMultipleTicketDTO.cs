using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.Ticket
{
    /// <summary>
    /// Use multiple tickets request
    /// </summary>
    /// <example>
    /// {
    ///     "productIds": [
    ///         1,
    ///         2
    ///         ]
    /// }
    /// </example>
    public class UseMultipleTicketDto
    {
        /// <summary>
        /// List of products ids to use a ticket for
        /// </summary>
        /// <value>Product Ids</value>
        /// <example>1, 2</example>
        [Required]
        public required List<int> ProductIds { get; set; } = new List<int>();
    }
}
