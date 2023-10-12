using System.Linq;
using CoffeeCard.Models.Entities;

public static class ProductUserGroupUtils
{
    /// <summary>
    /// Determines whether a product is a perk or not.
    /// A product is a perk if it is not available for regular customers.
    /// </summary>
    /// <param name="context">The DbContext to use for querying the database.</param>
    /// <param name="product">The product to check.</param>
    /// <returns>True if the product is a perk, false otherwise.</returns>
    public static bool isPerk(this Product product)
    {
        return product.ProductUserGroup.Any(pug => pug.UserGroup != UserGroup.Customer);
    }
}
