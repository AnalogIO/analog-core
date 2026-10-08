using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Payment details
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "discriminator")]
    [JsonDerivedType(typeof(MobilePayPaymentDetails), typeDiscriminator: "MobilePayPaymentDetails")]
    [JsonDerivedType(
        typeof(FreePurchasePaymentDetails),
        typeDiscriminator: "FreePurchasePaymentDetails"
    )]
    [JsonDerivedType(typeof(NexiPaymentDetails), typeDiscriminator: "NexiPaymentDetails")]
    public abstract class PaymentDetails
    {
        /// <summary>
        /// Payment type
        /// </summary>
        /// <example>MobilePay</example>
        [Required]
        public abstract PaymentType PaymentType { get; }

        /// <summary>
        /// Order id of purchase
        /// </summary>
        /// <example>f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c</example>
        [Required]
        public required string OrderId { get; set; } = string.Empty;
    }
}
