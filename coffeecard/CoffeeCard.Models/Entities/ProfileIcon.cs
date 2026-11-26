namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents the icons available for a user's profile picture.
    /// </summary>
    public enum ProfileIcon
    {
        // The explicit values are persisted in the database. Never change or
        // renumber existing members; only append new icons with the next unused value.
        CoffeeCup = 0,
        MokkaPot = 1,
        DripCoffee = 2,
        TeaCup = 3,
        MilkCarton = 4,
        PortaFilter = 5,
        IcedDrink = 6,
        FilterMachine = 7,
        CoffeeGrinder = 8,
        Teabag = 9,
    }
}
