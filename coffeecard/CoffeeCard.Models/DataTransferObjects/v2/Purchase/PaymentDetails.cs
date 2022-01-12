﻿using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    public abstract class PaymentDetails
    {
        /// <summary>
        /// Payment type
        /// </summary>
        /// <example>MobilePay</example>
        [Required]
        public PaymentType PaymentType { get; set; }
        
        /// <summary>
        /// Order id of purchase
        /// </summary>
        /// <example>f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c</example>
        [Required]
        public string OrderId { get; set; }
    }
}