using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.Entities
{
    public sealed class Product : IEquatable<Product>
    {
        public int Id { get; set; }

        public int Price { get; set; }

        public int NumberOfTickets { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ExperienceWorth { get; set; }

        [DefaultValue(true)]
        public bool Visible { get; set; } = true;

        public ICollection<ProductUserGroup> ProductUserGroup { get; set; }

        public bool Equals(Product? other)
        {
            return other != null && Id == other.Id && Price == other.Price && NumberOfTickets == other.NumberOfTickets &&
                   Name == other.Name && Description == other.Description && ExperienceWorth == other.ExperienceWorth &&
                   Visible == other.Visible;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Price, NumberOfTickets, Name, Description, ExperienceWorth, Visible);
        }
    }
}