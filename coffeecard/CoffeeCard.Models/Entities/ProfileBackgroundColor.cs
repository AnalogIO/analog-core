namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents the background colors available for a user's profile picture.
    /// </summary>
    public enum ProfileBackgroundColor
    {
        // Switching the order of enums is a breaking change since the index value is stored in the database
        LavenderPink,
        MintGreen,
        SageGreen,
        Periwinkle,
        DustyRose,
        Seafoam,
        BlushPink,
        Aqua,
        MossGreen,
        SteelBlue,
    }
}
