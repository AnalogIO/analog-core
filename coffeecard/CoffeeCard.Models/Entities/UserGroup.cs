using CoffeeCard.Models.DataTransferObjects.v2.User;

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

    public static class UserGroupExtention
    {
        public static UserRole toUserRole(this UserGroup userGroup)
        {
            return userGroup switch
            {
                UserGroup.Customer => UserRole.Customer,
                UserGroup.Barista => UserRole.Barista,
                UserGroup.Board => UserRole.Board,
                UserGroup.Manager => UserRole.Manager,
                _ => UserRole.Customer,
            };
        }
    }
}