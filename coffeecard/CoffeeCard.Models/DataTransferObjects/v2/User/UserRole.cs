namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// Corresponds to UserRole in the Entities, only serves to not expose the datamodel directly
    public enum UserRole
    {
        /// Normal user for customers
        Customer,

        /// Active Barista in Analog
        Barista,

        /// Active Manager in Analog
        Manager,

        /// Active board member in Analog
        Board,
    }
}
