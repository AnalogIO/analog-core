using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Initiate a new product add request
    /// </summary>
    public class AddProductRequest
    {
        [Required]
        public int Price { get; set; }

        [Required]
        public int NumberOfTickets { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [DefaultValue(true)]
        public bool Visible { get; set; } = true;
    }
}