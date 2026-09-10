namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents the icons available for a user's profile picture.
    /// </summary>
    public enum ProfileIcon
    {
        // Switching the order of enums is a breaking change since the index value is stored in the database
        CoffeeCup,
        MokkaPot,
        DripCoffee,
        TeaCup,
        MilkCarton,
        PortaFilter,
        IcedDrink,
        FilterMachine,
        CoffeeGrinder,
        Teabag,
    }
}
