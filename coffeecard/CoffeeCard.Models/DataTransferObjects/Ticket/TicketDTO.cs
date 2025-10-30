using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CoffeeCard.Models.DataTransferObjects.Ticket
{
    /// <summary>
    /// Representing a ticket for a product
    /// </summary>
    /// <example>
    /// {
    ///     "id": 122,
    ///     "dateCreated": "2022-01-09T21:03:52.2283208Z",
    ///     "dateUsed": "2022-01-09T21:03:52.2283208Z",
    ///     "productName: "Coffee"
    /// }
    /// </example>
    public class TicketDto
    {
        /// <summary>
        /// Ticket Id
        /// </summary>
        /// <value>Ticket Id</value>
        /// <example>122</example>
        [Required]
        public required int Id { get; set; }

        /// <summary>
        /// Issuing date time for ticket in Utc format
        /// </summary>
        /// <value>Issued date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public required DateTime DateCreated { get; set; }

        /// <summary>
        /// Used date time for ticket in Utc format
        /// </summary>
        /// <value>Used date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [JsonProperty(Required = Required.AllowNull)]
        public required DateTime? DateUsed { get; set; }

        /// <summary>
        /// Name of product a ticket is for
        /// </summary>
        /// <value>Product name</value>
        /// <example>Coffee</example>
        [Required]
        public required string ProductName { get; set; } = string.Empty;
    }
}
