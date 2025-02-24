using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Response object to a purchase request containing purchase information and reference to payment provider
    /// </summary>
    /// <example>
    /// {
    ///     "id": 122,
    ///     "dateCreated": "",
    ///     "productId": 1,
    ///     "productName": "Coffee",
    ///     "totalAmount": 100,
    ///     "purchaseStatus": "PendingPayment",
    ///     "paymentDetails": {
    ///         "paymentType": "MobilePay",
    ///         "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c",
    ///         "mobilePayAppRedirectUri": "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7",
    ///         "paymentId": "186d2b31-ff25-4414-9fd1-bfe9807fa8b7"
    ///     }
    /// }
    /// </example>
    public class InitiatePurchaseResponse
    {
        /// <summary>
        /// Id of the purchase
        /// </summary>
        /// <value>Purchase Id</value>
        /// <example>122</example>
        [Required]
        public required int Id { get; set; }

        /// <summary>
        /// Date time for purchase in Utc format
        /// </summary>
        /// <value>Purchase date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public required DateTime DateCreated { get; set; }

        /// <summary>
        /// Id of the product to be purchased
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }

        /// <summary>
        /// Name of the product to be purchased
        /// </summary>
        /// <value>Coffee</value>
        /// <example>Coffee</example>
        [Required]
        public required string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The amount of money to be collected by the purchase.
        /// A positive integer representing how much to charge in the smallest currency unit (e.g., 100 ører to charge 1.00 Danish kroner).
        /// </summary>
        /// <value>Total Amount in smallest currency unit</value>
        /// <example>100</example>
        [Required]
        public required int TotalAmount { get; set; }

        /// <summary>
        /// Status of the purchase
        /// </summary>
        /// <value>Purchase status</value>
        /// <example>Completed</example>
        [Required]
        public required PurchaseStatus PurchaseStatus { get; set; }

        /// <summary>
        /// Details about the payment. The details object is specific to the Payment Type
        /// </summary>
        /// <value>Payment Details</value>
        [Required]
        public required PaymentDetails PaymentDetails { get; set; }
    }
}
