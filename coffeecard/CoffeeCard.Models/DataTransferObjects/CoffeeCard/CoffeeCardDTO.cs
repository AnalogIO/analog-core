using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.CoffeeCard
{
    /// <summary>
    /// A CoffeeCard is a union datatype of a product and unused tickets associated with the product.
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1,
    ///     "name": "Filter Coffee",
    ///     "ticketsLeft": 5,
    ///     "price": 50,
    ///     "quantity": 10
    /// }
    /// </example>
    public class CoffeeCardDto
    {
        /// <summary>
        /// Id of product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }

        /// <summary>
        /// Name of product
        /// </summary>
        /// <value>Product Name</value>
        /// <example>Filter Coffee</example>
        [Required]
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Remaining (if any) unused tickets left for product
        /// </summary>
        /// <value>Remaining Tickets</value>
        /// <example>5</example>
        [Required]
        public required int TicketsLeft { get; set; }

        /// <summary>
        /// Price of product
        /// </summary>
        /// <value>Product Price</value>
        /// <example>50</example>
        [Required]
        public required int Price { get; set; }

        /// <summary>
        /// Quantity of tickets in product
        /// </summary>
        /// <value>Quantity</value>
        /// <example>10</example>
        [Required]
        public required int Quantity { get; set; }
    }
}
