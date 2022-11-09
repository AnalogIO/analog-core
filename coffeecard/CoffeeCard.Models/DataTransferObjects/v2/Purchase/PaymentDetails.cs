using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NJsonSchema.Converters;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    [KnownType(typeof(MobilePayPaymentDetails))]
    [JsonConverter(typeof(JsonInheritanceConverter))]
    public abstract class PaymentDetails
    {
        /// <summary>
        /// Payment type
        /// </summary>
        /// <example>MobilePay</example>
        [Required]
        public PaymentType PaymentType { get; set; }
        
        /// <summary>
        /// Order id of purchase
        /// </summary>
        /// <example>f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c</example>
        [Required]
        public string OrderId { get; set; }
        
        /// <summary>
        /// Id for a payment
        /// </summary>
        /// <example>186d2b31-ff25-4414-9fd1-bfe9807fa8b7</example>
        [Required]
        public string PaymentId { get; set; }

        /// <summary>
        /// The status of the payment for the purchase
        /// </summary>
        /// <example>PurchaseStatus.PendingPayment</example>
        [Required]
        public PurchaseStatus PurchaseStatus { get; set; }

    }
}