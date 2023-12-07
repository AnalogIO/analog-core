using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// /// <summary>
    /// Initiate an update product request.
    /// </summary>
    /// <example>
    /// {
    ///   "Id": 1,
    ///   "Price": 150,
    ///   "NumberOfTickets": 10,
    ///   "Name": "Espresso",
    ///   "Description": "A coffee made by forcing steam through ground coffee beans.",
    ///   "Visible": false,
    ///   "AllowedUserGroups": ["Manager", "Board"]
    /// }
    /// </example>
    public class UpdateProductRequest
    {
        /// <summary>
        /// Gets or sets the ID of the product to update.
        /// </summary>
        /// <value> Product Id </value>
        /// <example> 1 </example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the updated price of the product.
        /// </summary>
        /// <value> Product Price </value>
        /// <example> 10 </example>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a non-negative integer.")]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the updated number of tickets associated with the product.
        /// </summary>
        /// <value> Number of Tickets of a Product </value>
        /// <example> 5 </example>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of Tickets must be a non-negative integer.")]
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Gets or sets the updated name of the product.
        /// </summary>
        /// <value> Product Name </value>
        /// <example> Espresso </example>
        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be an empty string.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the updated description of the product.
        /// </summary>
        /// <value> Product Description </value>
        /// <example> A homemade espresso from fresh beans </example>
        [Required]
        [MinLength(1, ErrorMessage = "Description cannot be an empty string.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the updated visibility of the product. Default is true.
        /// </summary>
        /// <value> Product Visibility </value>
        /// <example> true </example>
        [DefaultValue(true)]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the user groups that can access the product.
        /// </summary>
        /// <value> Product User Groups </value>
        /// <example> Manager, Board </example>
        [Required]
        public IEnumerable<UserGroup> AllowedUserGroups { get; set; } = new List<UserGroup>();
    }
}