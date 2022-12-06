﻿using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CoffeeCard.Models.DataTransferObjects.v2.Ticket
{
    /// <summary>
    /// Representing a ticket for a product
    /// </summary>
    /// <example>
    /// {
    ///     "id": 122,
    ///     "dateCreated": "2022-01-09T21:03:52.2283208Z",
    ///     "dateUsed": "2022-01-09T21:03:52.2283208Z",
    ///     "productId": 1,
    ///     "productName: "Coffee"
    /// }
    /// </example>
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
        public string ProductName { get; set; }
    }
}