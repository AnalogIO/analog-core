using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NJsonSchema.Converters;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Payment details for a free purchase
    /// </summary>
    /// <example>
    /// {
    ///     "paymentType": "FreeProduct",
    ///     "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c",
    /// }
    /// </example>
    [KnownType(typeof(MobilePayPaymentDetails))]
    [JsonConverter(typeof(JsonInheritanceConverter))]
    public class FreeProductPaymentDetails : PaymentDetails
    {
        /// <summary>
        /// Creates a new instance of <see cref="FreeProductPaymentDetails"/>
        /// </summary>
        /// <param name="orderId"></param>
        public FreeProductPaymentDetails(string orderId)
        {
            PaymentType = PaymentType.Free;
            OrderId = orderId;
            PurchaseStatus = PurchaseStatus.Completed;
            PaymentId = "Free product - App";
        }
    }
}