using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product
{
    /// <summary>
    /// Initiate a new product request
    /// </summary>
    public class InitiateProductRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int Price { get; set; }

        public int NumberOfTickets { get; set; }

        [Required]
        public string Name { get; set; }


        [Required]
        public string Description { get; set; }

        public int ExperienceWorth { get; set; }

        [DefaultValue(true)]
        public bool Visible { get; set; } = true;
    }
}   