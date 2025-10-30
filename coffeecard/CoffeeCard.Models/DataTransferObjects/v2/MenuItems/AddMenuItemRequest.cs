using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.MenuItems
{
    /// <summary>
    /// Initiate a new menuitem add request.
    /// </summary>
    public class AddMenuItemRequest
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>Product Name</value>
        /// <example>Latte</example>
        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be an empty string")]
        public required string Name { get; set; }
    }
}
