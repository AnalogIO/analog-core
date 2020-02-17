using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int NumberOfTickets { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public int ExperienceWorth { get; set; }
        [Required]
        public bool Visible { get; set; }
        [Required]
        public bool BaristasOnly { get; set; }
    }
}