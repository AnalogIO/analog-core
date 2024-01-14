﻿using System.ComponentModel.DataAnnotations;

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
    }
}
