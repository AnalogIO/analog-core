using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NJsonSchema.Converters;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Staff payment details for free coffee
    /// </summary>
    /// <example>
    /// {
    ///     "paymentType": "StaffPerk",
    ///     "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c",
    /// }
    /// </example>
    [KnownType(typeof(MobilePayPaymentDetails))]
    [JsonConverter(typeof(JsonInheritanceConverter))]
    public class StaffPaymentDetails : PaymentDetails
    {
        /// <summary>
        /// Creates a new instance of <see cref="StaffPaymentDetails"/>
        /// </summary>
        /// <param name="orderId"></param>
        public StaffPaymentDetails(string orderId)
        {
            PaymentType = PaymentType.StaffPerk;
            OrderId = orderId;
        }
    }
}