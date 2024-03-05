using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Ticket
{
    /// <summary>
    /// Representing a used ticket for a product
    /// </summary>
    public class UsedTicketEvent
    {
        /// <summary>
        /// Used date time for ticket in Utc format
        /// </summary>
        /// <value>Used date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public required DateTime DateUsed { get; set; }

        /// <summary>
        /// Name of product a ticket is for
        /// </summary>
        /// <value>Product name</value>
        /// <example>Small drink</example>
        [Required]
        public required string ProductName { get; set; }

        /// <summary>
        /// Name of the menu item that this ticket was used on
        /// </summary>
        /// <value>Menu item name</value>
        /// <example>Cappuccino</example>
        /// <remarks>Null if ticket was not used on a menu item</remarks>
        public string? MenuItemName { get; set; }
        
        /// <summary>
        /// Name of the user who swiped the ticket
        /// </summary>
        /// <value>User name</value>
        /// <example>John Doe</example>
        [Required]
        public required string UserName { get; set; }
    }
}