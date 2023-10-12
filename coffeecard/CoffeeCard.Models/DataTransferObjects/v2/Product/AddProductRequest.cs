using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Initiate a new product add request.
    /// </summary>
    /// <example>
    /// {
    ///   "Price": 100,
    ///   "NumberOfTickets": 5,
    ///   "Name": "Latte",
    ///   "Description": "This is a delicious vegan latte made with premium beans.",
    ///   "Visible": true
    /// }
    /// </example>
    public class AddProductRequest
    {
        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        [Required]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the number of tickets associated with the product.
        /// </summary>
        [Required]
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the product. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Visible { get; set; } = true;
    }
}