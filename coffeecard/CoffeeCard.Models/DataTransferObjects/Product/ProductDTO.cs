using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.Product
{
    /// <summary>
    /// Represents a purchasable product
    /// </summary>
    /// <example>
    /// {
    ///     "id": 1,
    ///     "price": 300,
    ///     "numberOfTickets": 10,
    ///     "name": "Coffee clip card",
    ///     "description": "Coffee clip card of 10 clips"
    /// }
    /// </example>
    public class ProductDto
    {
        /// <summary>
        /// Id of product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int Id { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        /// <value>Product price</value>
        /// <example>300</example>
        [Required]
        public required int Price { get; set; }

        /// <summary>
        /// Number of tickets in product
        /// </summary>
        /// <value>Number of tickets</value>
        /// <example>10</example>
        [Required]
        public required int NumberOfTickets { get; set; }

        /// <summary>
        /// Name of product
        /// </summary>
        /// <value>Product name</value>
        /// <example>Coffee clip card</example>
        [Required]
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of products
        /// </summary>
        /// <value>Product Description</value>
        /// <example>Coffee clip card of 10 clips</example>
        [Required]
        public required string Description { get; set; } = string.Empty;
    }
}
