using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Represents the product response.
    /// </summary>
    public class ChangedProductResponse
    {
        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        /// <example> 150 </example>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a non-negative integer.")]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the number of tickets associated with the product.
        /// </summary>
        /// <value> Number of Tickets of a Product </value>
        /// <example> 5 </example>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of tickets must be a non-negative integer.")]
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value> Product Name </value>
        /// <example> Espresso </example>
        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be an empty string.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        /// <value> Product Description </value>
        /// <example> A homemade espresso from fresh beans </example>
        [Required]
        [MinLength(1, ErrorMessage = "Description cannot be an empty string.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the product.
        /// </summary>
        /// <value> Product Visibility </value>
        /// <example> true </example>
        [Required]
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the user groups that can access the product.
        /// </summary>
        /// <value> Product User Groups </value>
        /// <example> Manager, Board </example>
        [Required]
        public IEnumerable<UserGroup> AllowedUserGroups { get; set; } = new List<UserGroup>();

        /// <summary>
        /// Gets or sets the eligibible menu items for the product.
        /// </summary>
        /// <value> Product Menu Items </value>
        /// <example> Espresso, Cappuccino </example>
        [Required]
        public IEnumerable<MenuItemResponse> MenuItems { get; set; } = new List<MenuItemResponse>();
    }
}