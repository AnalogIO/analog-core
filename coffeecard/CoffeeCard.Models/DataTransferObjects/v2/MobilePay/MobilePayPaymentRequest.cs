using System;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay
{
    /// <summary>
    /// A request to initiate a payment with MobilePay
    /// </summary>
    public class MobilePayPaymentRequest
    {
        /// <summary>
        /// The amount to pay
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// The order id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// The description of the order
        /// </summary>
        public string Description { get; set; }
    }
}