using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.Purchase
{
    /// <summary>
    /// Represents a purchase
    /// </summary>
    /// <example>
    /// {
    ///     "id": 22,
    ///     "productName": "Coffee",
    ///     "productId": 1,
    ///     "price": 300,
    ///     "numberOfTickets": 10,
    ///     "dateCreated": "2022-01-09T21:03:52.2283208Z",
    ///     "completed: true,
    ///     "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c",
    ///     "transactionId": "1482981489"
    /// }
    /// </example>
    public class PurchaseDto
    {
        /// <summary>
        /// Id of purchase
        /// </summary>
        /// <value>Purchase id</value>
        /// <example>1371</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Name of purchased product
        /// </summary>
        /// <value>Product name</value>
        /// <example>Coffee</example>
        [Required]
        public required string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Id of purchased product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }

        /// <summary>
        /// Total purchase price in Danish Kroner (kr)
        /// </summary>
        /// <value>Total purchase price</value>
        /// <example>300</example>
        [Required]
        public required int Price { get; set; }

        /// <summary>
        /// Number of tickets issued in purchase
        /// </summary>
        /// <value>Num. Purchased Tickets</value>
        /// <example>10</example>
        [Required]
        public required int NumberOfTickets { get; set; }

        /// <summary>
        /// Date time for purchase in Utc format
        /// </summary>
        /// <value>Purchase date time</value>
        /// <example>2022-01-09T21:03:52.2283208Z</example>
        [Required]
        public required DateTime DateCreated { get; set; }

        /// <summary>
        /// Is purchase completed (with MobilePay)
        /// </summary>
        /// <value>Purchase completed</value>
        /// <example>true</example>
        [Required]
        public bool Completed { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        /// <value>Order id</value>
        /// <example>f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c</example>
        [Required]
        public required string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// Transaction id at external payment provider
        /// </summary>
        /// <value>Transaction Id</value>
        /// <example>1482981489</example>
        [Required]
        public required string TransactionId { get; set; } = string.Empty;
    }
}
