namespace CoffeeCard.Models
{
    public enum UserGroup
    {
        // Switching the order of enums is a breaking change since the index value is store in the database
        Customer,
        Barista,
        Manager,
        Board
    }
}