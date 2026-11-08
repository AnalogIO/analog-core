namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents the background colors available for a user's profile picture.
    /// </summary>
    public enum ProfileBackgroundColor
    {
        // The explicit values are persisted in the database. Never change or
        // renumber existing members; only append new colors with the next unused value.
        LavenderPink = 0,
        MintGreen = 1,
        SageGreen = 2,
        Periwinkle = 3,
        DustyRose = 4,
        Seafoam = 5,
        BlushPink = 6,
        Aqua = 7,
        MossGreen = 8,
        SteelBlue = 9,
    }
}
