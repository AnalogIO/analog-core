using System.Linq;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class ProductUserGroupUtils
{
    /// <summary>
    /// Determines whether a product is a perk or not.
    /// A product is a perk if it is not available for regular customers.
    /// </summary>
    /// <param name="context">The DbContext to use for querying the database.</param>
    /// <param name="product">The product to check.</param>
    /// <returns>True if the product is a perk, false otherwise.</returns>
    public static bool isPerk(DbContext context, Product product)
    {
        return context
            .Entry(product)
            .Collection(p => p.ProductUserGroup)
            .Query()
            .Include(x => x.UserGroup)
            .All(x => x.UserGroup != UserGroup.Customer);
    }
}
