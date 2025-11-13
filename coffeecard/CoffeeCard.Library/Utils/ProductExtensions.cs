using System;
using System.Linq;
using CoffeeCard.Models.DataTransferObjects.v2.MenuItems;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
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
                throw new ArgumentNullException(
                    $"{nameof(Product.ProductUserGroup)} must not be null"
                );
            }

            return product.ProductUserGroup.All(pug => pug.UserGroup != UserGroup.Customer);
        }

        public static ProductResponse ToProductResponse(this Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                NumberOfTickets = product.NumberOfTickets,
                Price = product.Price,
                IsPerk = product.IsPerk(),
                Visible = product.Visible,
                AllowedUserGroups = product.ProductUserGroup.Select(pug => pug.UserGroup),
                EligibleMenuItems = product.EligibleMenuItems.Select(mi => new MenuItemResponse
                {
                    Id = mi.Id,
                    Name = mi.Name,
                    Active = mi.Active,
                }),
            };
        }
    }
}
