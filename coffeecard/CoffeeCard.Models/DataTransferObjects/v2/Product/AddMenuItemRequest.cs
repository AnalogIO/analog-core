using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Initiate a new menuitem add request.
    /// </summary>
    /// <example>
    /// {
    ///     "Name": "Latte",
    /// }
    /// </example>
    public class AddMenuItemRequest
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value> Product Name </value>
        /// <example> Latte </example>
        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be an empty string.")]
        public string Name { get; set; }
    }
}