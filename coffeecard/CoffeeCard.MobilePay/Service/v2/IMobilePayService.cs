using System;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;

namespace CoffeeCard.MobilePay.Service.v2
{
    public interface IMobilePayService
    {
        /// <summary>
        /// Initiate MobilePay Payment
        /// </summary>
        /// <param name="paymentRequest">Payment request</param>
        /// <returns>Payment details to complete payment request</returns>
        /// <exception cref="MobilePayException">Initiation failed</exception>
        Task<MobilePayPaymentDetails> InitiatePayment(MobilePayPaymentRequest paymentRequest);

        /// <summary>
        /// Get Payment information
        /// </summary>
        /// <param name="paymentId">Payment Id</param>
        /// <returns>Payment information</returns>
        /// <exception cref="MobilePayException">Request failed</exception>
        Task<MobilePayPaymentDetails> GetPayment(Guid paymentId);

        /// <summary>
        /// Capture an already reserved payment
        /// </summary>
        /// <param name="paymentId">Payment Id</param>
        /// <param name="amountInDanishKroner">Amount to capture in Danish Kroner</param>
        /// <exception cref="MobilePayException">Capture failed</exception>
        Task CapturePayment(Guid paymentId, int amountInDanishKroner);

        /// <summary>
        /// Cancel a non-captured payment reservation
        /// </summary>
        /// <param name="paymentId">Payment Id</param>
        /// <exception cref="MobilePayException">Cancellation failed</exception>
        Task CancelPayment(Guid paymentId);

        /// <summary>
        /// Register Webhook at MobilePay
        /// </summary>
        /// <returns>Response with Webhook Id</returns>
        Task<SingleWebhookResponse> RegisterWebhook();

        /// <summary>
        /// Deregister Webhook at MobilePay
        /// </summary>
        /// <param name="webhookId">Existing Webhook Id</param>
        Task DeregisterWebhook(Guid webhookId);
    }
}