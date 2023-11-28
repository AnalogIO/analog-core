using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Products
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
    ///     "description": "Coffee clip card of 10 clips",
    ///     "isPerk": true,
    ///     "AllowedUserGroups": ["Manager", "Board"]
    /// }
    /// </example>
    public class ProductResponse
    {
        /// <summary>
        /// Id of product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        /// <value>Product price</value>
        /// <example>300</example>
        [Required]
        public int Price { get; set; }

        /// <summary>
        /// Number of tickets in product
        /// </summary>
        /// <value>Number of tickets</value>
        /// <example>10</example>
        [Required]
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Name of product
        /// </summary>
        /// <value>Product name</value>
        /// <example>Coffee clip card</example>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of products
        /// </summary>
        /// <value>Product Description</value>
        /// <example>Coffee clip card of 10 clips</example>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Eligible due to a user perk privilege 
        /// </summary>
        /// <value>Perk product</value>
        /// <example>true</example>
        [Required]
        public bool IsPerk { get; set; }

        /// <summary>
        /// Decides the user groups that can access the product.
        /// </summary>
        /// <value> Product User Groups </value>
        /// <example> Manager, Board </example>
        [Required]
        public IEnumerable<UserGroup> AllowedUserGroups { get; set; } = new List<UserGroup>();
    }
}
