using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Initiate a new product add request.
    /// </summary>
    public class AddProductRequest
    {
        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a non-negative integer.")]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the number of tickets associated with the product.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Number of Tickets must be a non-negative integer.")]
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Name cannot be an empty string.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Description cannot be an empty string.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the product. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Visible { get; set; } = true;

        [Required]
        public IEnumerable<UserGroup> AllowedUserGroups { get; set; }

    }
}