using System;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2.PaymentStrategies
{
    /// <summary>
    /// Payment strategy for MobilePay payments.
    /// </summary>
    public sealed class MobilePayPaymentStrategy : IPaymentStrategy
    {
        private readonly IMobilePayPaymentsService _mobilePayPaymentsService;

        public MobilePayPaymentStrategy(IMobilePayPaymentsService mobilePayPaymentsService)
        {
            _mobilePayPaymentsService = mobilePayPaymentsService;
        }

        public async Task<PaymentInitiationResult> InitiatePaymentAsync(
            ProductResponse product,
            Guid orderId
        )
        {
            var paymentDetails = await _mobilePayPaymentsService.InitiatePayment(
                new MobilePayPaymentRequest
                {
                    Amount = product.Price,
                    OrderId = orderId,
                    Description = product.Name,
                }
            );

            var transactionId = ((MobilePayPaymentDetails)paymentDetails).PaymentId;

            return new PaymentInitiationResult(
                PurchaseStatus: PurchaseStatus.PendingPayment,
                TransactionId: transactionId,
                PaymentDetails: paymentDetails
            );
        }

        public async Task<PaymentDetails> GetPaymentAsync(Purchase purchase)
        {
            return await _mobilePayPaymentsService.GetPayment(GetPaymentId(purchase));
        }

        public Task CapturePaymentAsync(Purchase purchase)
        {
            return _mobilePayPaymentsService.CapturePayment(
                GetPaymentId(purchase),
                purchase.Price
            );
        }

        public Task CancelPaymentAsync(Purchase purchase)
        {
            return _mobilePayPaymentsService.CancelPayment(GetPaymentId(purchase));
        }

        public Task<bool> RefundPaymentAsync(Purchase purchase)
        {
            // MobilePay expects refund amount in øre; purchases store the price in kroner.
            return _mobilePayPaymentsService.RefundPayment(purchase, purchase.Price * 100);
        }

        private static Guid GetPaymentId(Purchase purchase)
        {
            if (
                purchase.ExternalTransactionId == null
                || !Guid.TryParse(purchase.ExternalTransactionId, out var paymentId)
            )
            {
                throw new InvalidOperationException(
                    $"Purchase {purchase.Id} does not have a valid payment id"
                );
            }

            return paymentId;
        }
    }
}
