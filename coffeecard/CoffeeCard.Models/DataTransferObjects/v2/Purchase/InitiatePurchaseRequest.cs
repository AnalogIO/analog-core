using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Initiate a new purchase request
    /// </summary>
    public class InitiatePurchaseRequest
    {
        /// <summary>
        /// Id of product to be purchased
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// Payment Type used to fulfill purchase
        /// </summary>
        /// <value>Payment Type</value>
        /// <example>MobilePay</example>
        [Required]
        public PaymentType PaymentType { get; set; }
    }
}