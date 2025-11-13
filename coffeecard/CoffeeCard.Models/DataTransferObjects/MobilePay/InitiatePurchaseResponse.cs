using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay
{
    /// <summary>
    /// Initiation of a new purchase response
    /// </summary>
    /// <example>
    /// {
    ///     "orderId": "ae76a5ba-82e8-46d8-8431-6cbb3130b94a"
    /// }
    /// </example>
    public class InitiatePurchaseResponse
    {
        /// <summary>
        /// Order Id
        /// </summary>
        /// <value>Order id</value>
        /// <example>ae76a5ba-82e8-46d8-8431-6cbb3130b94a</example>
        [Required]
        public required string OrderId { get; set; } = string.Empty;
    }
}
