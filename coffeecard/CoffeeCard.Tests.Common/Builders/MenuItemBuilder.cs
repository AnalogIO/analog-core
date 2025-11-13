using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders;

[BuilderFor(typeof(MenuItem))]
public partial class MenuItemBuilder
{
    public static MenuItemBuilder Simple()
    {
        return new MenuItemBuilder()
            .WithName(f => f.Commerce.Product())
            .WithActive(true)
            .WithMenuItemProducts([])
            .WithAssociatedProducts([]);
    }

    public static MenuItemBuilder Typical()
    {
        return Simple();
    }
}
