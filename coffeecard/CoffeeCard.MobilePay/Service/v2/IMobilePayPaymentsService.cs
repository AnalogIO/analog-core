using System;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.MobilePay.Service.v2
{
    public interface IMobilePayPaymentsService
    {
        /// <summary>
        /// Initiate MobilePay Payment
        /// </summary>
        /// <param name="paymentRequest">Payment request</param>
        /// <returns>Payment details to complete payment request</returns>
        /// <exception cref="MobilePayApiException">Initiation failed</exception>
        Task<MobilePayPaymentDetails> InitiatePayment(MobilePayPaymentRequest paymentRequest);

        /// <summary>
        /// Get Payment information
        /// </summary>
        /// <param name="paymentId">Payment Id</param>
        /// <returns>Payment information</returns>
        /// <exception cref="MobilePayApiException">Request failed</exception>
        Task<MobilePayPaymentDetails> GetPayment(Guid paymentId);

        /// <summary>
        /// Capture an already reserved payment
        /// </summary>
        /// <param name="paymentId">Payment Id</param>
        /// <param name="amountInDanishKroner">Amount to capture in Danish Kroner</param>
        /// <exception cref="MobilePayApiException">Capture failed</exception>
        Task CapturePayment(Guid paymentId, int amountInDanishKroner);

        /// <summary>
        /// Cancel a non-captured payment reservation
        /// </summary>
        /// <param name="paymentId">Payment Id</param>
        /// <exception cref="MobilePayApiException">Cancellation failed</exception>
        Task CancelPayment(Guid paymentId);

        /// <summary>
        /// Refund a payment to the customer
        /// </summary>
        /// <param name="purchase">Purchase to refund</param>
        /// <param name="amount">Amount to refund in oere</param>
        /// <returns>True if refund was successful, false otherwise</returns>
        Task<bool> RefundPayment(Purchase purchase, int amount);
    }
}
