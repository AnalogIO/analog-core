using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Represents a purchase
    /// </summary>
    /// <example>
    /// {
    ///     "id": 22,
    ///     "dateCreated": "2022-01-09T21:03:52.2283208Z",
    ///     "productId": 1,
    ///     "totalAmount": 300,
    ///     "purchaseStatus": "Completed",
    ///     "paymentDetails": {
    ///         "paymentType": "MobilePay",
    ///         "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c",
    ///         "mobilePayAppRedirectUri": "mobilepay://merchant_payments?payment_id=186d2b31-ff25-4414-9fd1-bfe9807fa8b7",
    ///         "paymentId": "186d2b31-ff25-4414-9fd1-bfe9807fa8b7"
    ///     }
    /// }
    /// </example>
    public class SinglePurchaseResponse
    {
        /// <summary>
        /// Id of purchase
        /// </summary>
        /// <value>Purchase id</value>
        /// <example>1371</example>
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
        /// Id of purchased product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }

        /// <summary>
        /// Total purchase price in Danish Kroner (kr)
        /// </summary>
        /// <value>Total purchase price</value>
        /// <example>300</example>
        [Required]
        public required int TotalAmount { get; set; }

        /// <summary>
        /// Status of the purchase
        /// </summary>
        /// <value>Purchase status</value>
        /// <example>Completed</example>
        [Required]
        public required PurchaseStatus? PurchaseStatus { get; set; }

        /// <summary>
        /// Details about the payment. The details object is specific to the Payment Type
        /// </summary>
        /// <value>Payment Details</value>
        [Required]
        public required PaymentDetails PaymentDetails { get; set; }
    }
}
