using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Update the UserGroup property of a user
    /// </summary>
    /// <example>
    /// {
    ///     "UserGroup": "Barista"
    /// }
    /// </example>
    public class UpdateUserGroupRequest
    {
        /// <summary>
        /// The UserGroup of a user
        /// </summary>
        /// <value> UserGroup object </value>
        /// <example> UserGroup.Barista </example>
        [Required]
        public UserGroup UserGroup { get; set; }
    }
}
