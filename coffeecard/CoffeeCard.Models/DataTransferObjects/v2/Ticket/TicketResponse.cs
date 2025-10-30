using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CoffeeCard.Models.DataTransferObjects.v2.Ticket
{
    /// <summary>
    /// Representing a ticket for a product
    /// </summary>
    public class TicketResponse
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
        [JsonProperty(Required = Required.AllowNull)]
        public DateTime? DateUsed { get; set; }

        /// <summary>
        /// The Id of product a ticket is for
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// Name of product a ticket is for
        /// </summary>
        /// <value>Product name</value>
        /// <example>Coffee</example>
        [Required]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the menu item that this ticket was used on
        /// </summary>
        /// <value>Menu item Id</value>
        /// <example>Cappuccino</example>
        /// <remarks>Null if ticket was not used on a menu item</remarks>
        public string? UsedOnMenuItemName { get; set; }
    }
}
