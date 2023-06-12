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

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayPaymentRequest"/> class.
        /// </summary>
        public MobilePayPaymentRequest(int amount, Guid orderId, string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Description cannot be null or empty", nameof(description));
            }

            Amount = amount;
            OrderId = orderId;
            Description = description;
        }
    }
}