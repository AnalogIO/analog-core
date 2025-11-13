using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay
{
    /// <summary>
    /// Initiate a new purchase request
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1
    /// }
    /// </example>
    public class InitiatePurchaseDto
    {
        /// <summary>
        /// Id of product for purchase request
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }
    }
}
