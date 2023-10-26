using System;
using System.Linq;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Utils
{
    public static class ProductExtensions
    {
        /// <summary>
        /// Determines whether a product is a perk or not.
        /// A product is a perk if it is not available for regular customers.
        /// </summary>
        /// <param name="product">The product to check.</param>
        /// <returns>True if the product is a perk, false otherwise.</returns>
        public static bool IsPerk(this Product product)
        {
            if (product.ProductUserGroup == null)
            {
                throw new NullReferenceException("Product User Group must not be null");
            }

            return product.ProductUserGroup.Any(pug => pug.UserGroup != UserGroup.Customer);
        }
    }
}
