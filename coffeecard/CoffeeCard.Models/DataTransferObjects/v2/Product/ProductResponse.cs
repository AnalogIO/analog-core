using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Initiate a new product request
    /// </summary>
    public class ProductResponse
    {
        [Required]
        public int Price { get; set; }

        public int NumberOfTickets { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public bool Visible { get; set; }
    }
}