using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public enum WebhookNotification
    {
        Created = 0,
        Aborted = 1,
        Expired = 2,
        Cancelled = 3,
        Captured = 4,
        Refunded = 5,
        Authorized = 6,
        Terminated = 7,
    }
    public interface IPurchaseService : IDisposable
    {
        /// <summary>
        /// Initiate a new purchase. Depending on the PaymentType, the purchase might be completed in the future
        /// </summary>
        /// <param name="initiateRequest">Initiate request with information on purchasable product and payment type</param>
        /// <param name="user">User</param>
        /// <returns>Response with Purchase details, status and payment details</returns>
        Task<InitiatePurchaseResponse> InitiatePurchase(
            InitiatePurchaseRequest initiateRequest,
            User user
        );

        /// <summary>
        /// Get purchase by Purchase Id
        /// </summary>
        /// <param name="purchaseId">Purchase Id</param>
        /// <param name="user">User</param>
        /// <returns>Purchase details</returns>
        Task<SinglePurchaseResponse> GetPurchase(int purchaseId, User user);

        /// <summary>
        /// Get all purchase for user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Purchase details</returns>
        Task<IEnumerable<SimplePurchaseResponse>> GetPurchases(User user);

        /// <summary>
        /// Get all purchases for a user by its Id
        /// </summary>
        /// <param name="userId">id of user to fetch purchases from</param>
        /// <returns>Purchase details for all users purchases</returns>
        Task<IEnumerable<SimplePurchaseResponse>> GetPurchases(int userId);

        /// <summary>
        /// Handle MobilePay webhook invocation and update purchase accordingly
        /// </summary>
        /// <param name="webhook">Webhook data object</param>
        /// <returns></returns>
        Task HandleMobilePayPaymentUpdate(MobilePay.Generated.Api.ePaymentApi.WebhookEvent webhook);

        /// <summary>
        /// Handles a normalized payment webhook notification.
        /// </summary>
        /// <param name="referenceId">The payment provider transaction reference.</param>
        /// <param name="notification">The normalized webhook notification.</param>
        /// <param name="paymentType">The payment provider that sent the notification.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleWebhookPaymentUpdate(
            string referenceId,
            WebhookNotification notification,
            PaymentType paymentType
        );

        /// <summary>
        /// Redeem af voucher code for a purchase
        /// </summary>
        /// <param name="voucherCode">Voucher code</param>
        /// <param name="user">user redeeming the voucher</param>
        Task<SimplePurchaseResponse> RedeemVoucher(string voucherCode, User user);

        /// <summary>
        /// Refund a purchase
        /// </summary>
        /// <param name="paymentId">The payment to refund</param>
        /// <returns>The purchase after being refunded</returns>
        Task<SimplePurchaseResponse> RefundPurchase(int paymentId);
    }
}
