using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents a product.
    /// </summary>
    public sealed class Product : IEquatable<Product>
    {
        /// <summary>
        /// Gets or sets the Id of the product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Price of the product.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the number of tickets associated with the product.
        /// </summary>
        public int NumberOfTickets { get; set; }

        /// <summary>
        /// Gets or sets the Name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the experience worth of the product.
        /// </summary>
        public int ExperienceWorth { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the product.
        /// </summary>
        [DefaultValue(true)]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the collection of user groups associated with the product.
        /// </summary>
        public ICollection<ProductUserGroup> ProductUserGroup { get; set; }

        /// <summary>
        /// The menu item(s) that this product is eligible to redeem
        /// </summary>
        public ICollection<MenuItem> EligibleMenuItems { get; set; }

        /// <summary>
        /// Navigational property for the join table between Menu Items and Products
        /// </summary>
        public ICollection<MenuItemProduct> MenuItemProducts { get; set; }

        public bool Equals(Product? other)
        {
            return other != null
                && Id == other.Id
                && Price == other.Price
                && NumberOfTickets == other.NumberOfTickets
                && Name == other.Name
                && Description == other.Description
                && ExperienceWorth == other.ExperienceWorth
                && Visible == other.Visible;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Product)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Price,
                NumberOfTickets,
                Name,
                Description,
                ExperienceWorth,
                Visible
            );
        }
    }
}
