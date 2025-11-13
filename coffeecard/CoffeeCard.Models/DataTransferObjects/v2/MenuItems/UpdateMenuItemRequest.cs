using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.MenuItems
{
    /// /// <summary>
    /// Initiate an update product request.
    /// </summary>
    public class UpdateMenuItemRequest
    {
        /// <summary>
        /// Gets or sets the updated name of the product.
        /// </summary>
        /// <value>Product Name</value>
        /// <example>Espresso</example>
        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be an empty string")]
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the updated active status of the product.
        /// </summary>
        /// <value>Product Active</value>
        /// <example>true</example>
        [Required]
        public required bool Active { get; set; }
    }
}
