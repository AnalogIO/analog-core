namespace CoffeeCard.Models.Entities
{
    public enum UserGroup
    {
        // Switching the order of enums is a breaking change since the index value is store in the database
        /// Normal user for customers
        Customer,
        /// Active Barista in Analog
        Barista, 
        /// Active Manager in Analog
        Manager,
        /// Active board member in Analog
        Board 
    }
}