using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Models.DataTransferObjects.CoffeeCard
{
    /// <summary>
    /// A coffee card represents both a product and available tickets for user (if any) 
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1,
    ///     "name": "Coffee clip card"
    ///     "ticketsLeft": 5,
    ///     "price": 300,
    ///     "quantity": 10
    /// }
    /// </example>
    public class CoffeeCardDto
    {
        /// <summary>
        /// Product Id
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public int ProductId { get; set; }
        
        /// <summary>
        /// Product name
        /// </summary>
        /// <value>Product name</value>
        /// <example>Coffee clip card</example>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Unused tickets left for user (if any)
        /// </summary>
        /// <value>Tickets left</value>
        /// <example>2</example>
        [Required]
        public int TicketsLeft { get; set; }
        
        /// <summary>
        /// Product price
        /// </summary>
        /// <value>Product price</value>
        /// <example>300</example>
        [Required]
        public int Price { get; set; }
        
        /// <summary>
        /// Number of ticket issued by product
        /// </summary>
        /// <value>Quantity</value>
        /// <example>10</example>
        [Required]
        public int Quantity { get; set; }
    }
}