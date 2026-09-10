using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2.PaymentStrategies
{
    /// <summary>
    /// Encapsulates the payment-method-specific logic for a purchase.
    /// Register implementations as keyed services using <see cref="CoffeeCard.Models.DataTransferObjects.v2.Purchase.PaymentType"/> enum values as keys.
    /// </summary>
    public interface IPaymentStrategy
    {
        /// <summary>
        /// Initiates a payment for the given product with the provided order id.
        /// </summary>
        Task<PaymentInitiationResult> InitiatePaymentAsync(ProductResponse product, Guid orderId);

        /// <summary>
        /// Gets payment details for an existing purchase.
        /// </summary>
        Task<PaymentDetails> GetPaymentAsync(Purchase purchase);

        /// <summary>
        /// Captures an authorized payment for an existing purchase.
        /// </summary>
        Task CapturePaymentAsync(Purchase purchase);

        /// <summary>
        /// Cancels an authorized payment for an existing purchase.
        /// </summary>
        Task CancelPaymentAsync(Purchase purchase);

        /// <summary>
        /// Refunds an existing purchase.
        /// </summary>
        Task<bool> RefundPaymentAsync(Purchase purchase);
    }
}
