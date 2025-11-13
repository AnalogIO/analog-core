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
    ///     "productName": "Filter coffee",
    ///     "totalAmount": 300,
    ///     "purchaseStatus": "Completed"
    /// }
    /// </example>
    public class SimplePurchaseResponse
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
        /// Name of purchased product
        /// </summary>
        /// <value>Product Name</value>
        /// <example>1</example>
        [Required]
        public required string ProductName { get; set; }

        /// <summary>
        /// Number of tickets issued in purchase
        /// </summary>
        /// <value>Amount of tickets granted by the purchase</value>
        /// <example>10</example>
        [Required]
        public required int NumberOfTickets { get; set; }

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
        public required PurchaseStatus PurchaseStatus { get; set; }
    }
}
