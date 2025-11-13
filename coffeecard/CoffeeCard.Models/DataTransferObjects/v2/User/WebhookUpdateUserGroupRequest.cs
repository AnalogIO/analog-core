using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    /// <summary>
    /// Represents a request to update user groups in bulk
    /// </summary>
    public class WebhookUpdateUserGroupRequest
    {
        /// <summary>
        /// List of accounts and their new user groups
        /// </summary>
        [Required]
        public required IEnumerable<AccountUserGroup> PrivilegedUsers { get; set; }
    }
}
