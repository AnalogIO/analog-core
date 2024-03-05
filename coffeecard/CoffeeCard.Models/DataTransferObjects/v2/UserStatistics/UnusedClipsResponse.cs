using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.UserStatistics
{
    /// <summary>
    /// Initialize a response with unused clips data
    /// </summary>
    public class UnusedClipsResponse
    {
        /// <summary>
        /// The id of the product
        /// </summary>
        /// <value> Product Id </value>
        /// <example> 1 </example>
        [Required]
        public int ProductId { get; set; }


        /// <summary>
        /// The name of the product
        /// </summary>
        /// <value> Product Name </value>
        /// <example> Americano </example>
        [Required]
        public string ProductName { get; set; }

        /// <summary>
        /// The number of tickets unused in a purchase
        /// </summary>
        /// <value> Tickets left </value>
        /// <example> 8 </example>
        [Required]
        public int TicketsLeft { get; set; }

        /// <summary>
        /// The amount that should be refunded
        /// </summary>
        /// <value> Amount to refund </value>
        /// <example> 40.2 </example>
        [Required]
        public decimal ToRefund { get; set; }
    }
}

