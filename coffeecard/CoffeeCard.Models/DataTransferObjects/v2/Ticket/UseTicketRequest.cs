using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Ticket
{
    /// <summary>
    /// Represents a request to use a ticket.
    /// </summary>
    public class UseTicketRequest
    {
        /// <summary>
        /// The id of the product the ticket is for.
        /// </summary>
        /// <value>Product id</value>
        /// <example>1</example>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// The id of the menu item to use the ticket on.
        /// </summary>
        /// <value>Menu item id</value>
        /// <example>1</example>
        [Required]
        public int MenuItemId { get; set; }
    }
}
