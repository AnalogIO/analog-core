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
    ///     "purchaseStatus": "Completed"
    ///     
    /// }
    /// </example>
    public class Purchase
    {
        /// <summary>
        /// Id of purchase
        /// </summary>
        /// <value>Purchase id</value>
        /// <example>1371</example>
        [Required]
        public int Id { get; set; }
        
        /// <summary>
        /// Date time for purchase in Utc format
        /// </summary>
        /// <value>Purchase date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Id of purchased product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public int ProductId { get; set; }
        
        /// <summary>
        /// Total purchase price in Danish Kroner (kr)
        /// </summary>
        /// <value>Total purchase price</value>
        /// <example>300</example>
        [Required]
        public int TotalAmount { get; set; }
        
        /// <summary>
        /// Status of the purchase
        /// </summary>
        /// <value>Purchase status</value>
        /// <example>Completed</example>
        [Required]
        public PurchaseStatus PurchaseStatus { get; set; }
    }
}