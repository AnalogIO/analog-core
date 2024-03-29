using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the updated active status of the product.
        /// </summary>
        /// <value>Product Active</value>
        /// <example>true</example>
        [Required]
        public bool Active { get; set; }
    }
}