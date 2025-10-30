using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay
{
    /// <summary>
    /// Represents a request to complete a purchase
    /// </summary>
    /// <example>
    /// {
    ///     "orderId": "ae76a5ba-82e8-46d8-8431-6cbb3130b94a",
    ///     "transactionId": "123241619"
    /// }
    /// </example>
    public class CompletePurchaseDto
    {
        /// <summary>
        /// Order Id for purchase
        /// </summary>
        /// <value>Order id</value>
        /// <example>ae76a5ba-82e8-46d8-8431-6cbb3130b94a</example>
        [Required]
        public required string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// Transaction Id at external payment provider
        /// </summary>
        /// <value>Transaction id</value>
        /// <example>123241619</example>
        [Required]
        public required string TransactionId { get; set; } = string.Empty;
    }
}
