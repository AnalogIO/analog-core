using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.Ticket
{
    /// <summary>
    /// Use ticket request
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1
    /// }
    /// </example>
    public class UseTicketDTO
    {
        /// <summary>
        /// Id of product to use a ticket for
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// Id of an eligible menu item to use the ticket on
        /// </summary>
        /// <value>Menu item Id</value>
        /// <example>1</example>
        /// <remarks>Optional for backwards compatibility</remarks>
        public int? MenuItemId { get; set; }
    }
}