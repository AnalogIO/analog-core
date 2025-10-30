using CoffeeCard.Models.DataTransferObjects.v2.User;

namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents the different groups that a user can belong to.
    /// </summary>
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
        Board,
    }

    /// <summary>
    /// Extension methods for the UserGroup enum.
    /// </summary>
    public static class UserGroupExtention
    {
        /// <summary>
        /// Converts a UserGroup enum value to a UserRole enum value.
        /// </summary>
        /// <param name="userGroup">The UserGroup enum value to convert.</param>
        /// <returns>The corresponding UserRole enum value.</returns>
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
