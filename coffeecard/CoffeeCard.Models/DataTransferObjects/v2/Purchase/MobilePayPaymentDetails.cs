using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// MobilePay Payment details
    /// </summary>
    /// <example>
    /// {
    ///     "paymentType": "MobilePay",
    ///     "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c",
    ///     "mobilePayAppRedirectUri": "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7",
    ///     "paymentId": "186d2b31-ff25-4414-9fd1-bfe9807fa8b7"
    /// }
    /// </example>
    public class MobilePayPaymentDetails : PaymentDetails
    {
        /// <summary>
        /// App deeplink for a MobilePay payment
        /// </summary>
        /// <example>mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7</example>
        [Required]
        public string MobilePayAppRedirectUri { get; }

        /// <summary>
        /// MobilePay Id for a payment
        /// </summary>
        /// <example>186d2b31-ff25-4414-9fd1-bfe9807fa8b7</example>
        [Required]
        public string PaymentId { get; }

        /// <summary>
        /// Creates a new instance of <see cref="MobilePayPaymentDetails"/>
        /// </summary>
        public MobilePayPaymentDetails(string orderId, string mobilePayAppRedirectUri, string paymentId)
        {
            PaymentType = PaymentType.MobilePay;
            OrderId = orderId;
            MobilePayAppRedirectUri = mobilePayAppRedirectUri;
            PaymentId = paymentId;
        }
    }
}
