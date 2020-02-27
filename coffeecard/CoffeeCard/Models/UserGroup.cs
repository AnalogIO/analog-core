namespace CoffeeCard.WebApi.Models
{
    public enum UserGroup
    {
        // Switching the order of enums is a breaking change since the index value is store in the database
        Customer, // Normal user for customers
        Barista, // Active Barista in Analog
        Manager, // Active Manager in Analog
        Board // Active board member in Analog
    }
}