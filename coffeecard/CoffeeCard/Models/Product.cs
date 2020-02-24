using System;
using System.Collections.Generic;
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

        protected bool Equals(Product other)
        {
            return Id == other.Id && Price == other.Price && NumberOfTickets == other.NumberOfTickets && Name == other.Name && Description == other.Description && ExperienceWorth == other.ExperienceWorth && Visible == other.Visible && BaristasOnly == other.BaristasOnly;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product)obj);
        }
    }
}