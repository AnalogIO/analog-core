using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Ticket
{
    /// <summary>
    /// Representing a used ticket for a product
    /// </summary>
    /// <example>
    /// {
    ///     "id": 122,
    ///     "dateCreated": "2022-01-09T21:03:52.2283208Z",
    ///     "dateUsed": "2022-01-09T21:03:52.2283208Z",
    ///     "productName: "Coffee"
    /// }
    /// </example>
    public class UsedTicketResponse
    {
        /// <summary>
        /// Ticket Id
        /// </summary>
        /// <value>Ticket Id</value>
        /// <example>122</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Issuing date time for ticket in Utc format
        /// </summary>
        /// <value>Issued date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Used date time for ticket in Utc format
        /// </summary>
        /// <value>Used date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public DateTime DateUsed { get; set; }

        /// <summary>
        /// Name of product a ticket is for
        /// </summary>
        /// <value>Product name</value>
        /// <example>Small drink</example>
        [Required]
        public string ProductName { get; set; }

        /// <summary>
        /// Name of the menu item that this ticket was used on
        /// </summary>
        /// <value>Menu item name</value>
        /// <example>Cappuccino</example>
        /// <remarks>Null if ticket was not used on a menu item</remarks>
        public string? MenuItemName { get; set; }
    }
}