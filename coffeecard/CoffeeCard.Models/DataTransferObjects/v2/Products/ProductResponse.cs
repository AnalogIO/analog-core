using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.DataTransferObjects.v2.MenuItems;
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
    ///     "visible": true,
    ///     "allowedUserGroups": ["Manager", "Board"],
    ///     "eligibleMenuItems": [
    ///         { "id": 1, "name": "Cappuccino" },
    ///         { "id": 2, "name": "Caffe Latte" }
    ///     ]
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

        /// <summary>
        /// Eligible due to a user perk privilege
        /// </summary>
        /// <value>Perk product</value>
        /// <example>true</example>
        [Required]
        public required bool IsPerk { get; set; }

        /// <summary>
        /// Visibility of products for users
        /// </summary>
        /// <value>Product visibility</value>
        /// <example>true</example>
        [Required]
        public required bool Visible { get; set; }

        /// <summary>
        /// Decides the user groups that can access the product.
        /// </summary>
        /// <value> Product User Groups </value>
        /// <example> Manager, Board </example>
        [Required]
        public required IEnumerable<UserGroup> AllowedUserGroups { get; set; }

        /// <summary>
        /// The menu items that this product can be used on.
        /// </summary>
        /// <value>Menu items</value>
        /// <example>["Cappuccino", "Caffe Latte"]</example>
        /// <remarks>Optional for backwards compatibility</remarks>
        public required IEnumerable<MenuItemResponse> EligibleMenuItems { get; set; }
    }
}
