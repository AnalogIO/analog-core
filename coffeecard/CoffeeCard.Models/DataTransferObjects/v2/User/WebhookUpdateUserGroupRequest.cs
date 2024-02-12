using CoffeeCard.Models.Entities;
using System.Collections.Generic;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    public class WebhookUpdateUserGroupRequest
    {
        public string Hash { get; set; }

        // TODO Use custom object instead
        public IEnumerable<(int AccountId, UserGroup UserGroup)> PrivilegedUsers { get; set;}
    }
}
