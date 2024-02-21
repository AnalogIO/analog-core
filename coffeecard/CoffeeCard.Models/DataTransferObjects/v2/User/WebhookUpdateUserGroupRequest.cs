using CoffeeCard.Models.Entities;
using System.Collections.Generic;

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
        public IEnumerable<AccountUserGroup> PrivilegedUsers { get; set; }
    }
}
