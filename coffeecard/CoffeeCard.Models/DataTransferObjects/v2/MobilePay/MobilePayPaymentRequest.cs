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
        public required int Amount { get; set; }

        /// <summary>
        /// The order id
        /// </summary>
        public required Guid OrderId { get; set; }

        /// <summary>
        /// The description of the order
        /// </summary>
        public required string Description { get; set; }
    }
}
