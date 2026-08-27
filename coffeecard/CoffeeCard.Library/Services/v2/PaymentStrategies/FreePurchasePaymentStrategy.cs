using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2.PaymentStrategies
{
    /// <summary>
    /// Payment strategy for free (zero-cost) purchases.
    /// </summary>
    public sealed class FreePurchasePaymentStrategy : IPaymentStrategy
    {
        public Task<PaymentInitiationResult> InitiatePaymentAsync(
            ProductResponse product,
            Guid orderId
        )
        {
            var paymentDetails = new FreePurchasePaymentDetails(orderId.ToString());

            return Task.FromResult(
                new PaymentInitiationResult(
                    PurchaseStatus: PurchaseStatus.Completed,
                    TransactionId: null,
                    PaymentDetails: paymentDetails
                )
            );
        }

        public Task<PaymentDetails> GetPaymentAsync(Purchase purchase)
        {
            return Task.FromResult<PaymentDetails>(
                new FreePurchasePaymentDetails(purchase.OrderId)
            );
        }

        public Task CapturePaymentAsync(Purchase purchase) =>
            throw new InvalidOperationException("Free purchases cannot be captured");

        public Task CancelPaymentAsync(Purchase purchase) =>
            throw new InvalidOperationException("Free purchases cannot be cancelled");

        public Task<bool> RefundPaymentAsync(Purchase purchase) =>
            throw new InvalidOperationException("Free purchases cannot be refunded");
    }
}
