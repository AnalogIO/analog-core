using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    ///   "Visible": false
    /// }
    /// </example>
    public class UpdateProductRequest
    {
        /// <summary>
        /// Gets or sets the ID of the product to update.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the updated price of the product.
        /// </summary>
        [Required]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the updated number of tickets associated with the product.
        /// </summary>
        [Required]
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Gets or sets the updated name of the product.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the updated description of the product.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the updated visibility of the product. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool Visible { get; set; } = true;
    }
}