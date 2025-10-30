using CoffeeCard.Models.Entities;

namespace CoffeeCard.Tests.Common.Builders;

[BuilderFor(typeof(MenuItemProduct))]
public partial class MenuItemProductBuilder
{
    public static MenuItemProductBuilder Simple()
    {
        return new MenuItemProductBuilder()
            .WithProduct(ProductBuilder.Simple().Build())
            .WithMenuItem(MenuItemBuilder.Simple().Build());
    }

    public static MenuItemProductBuilder Typical()
    {
        return Simple();
    }
}
