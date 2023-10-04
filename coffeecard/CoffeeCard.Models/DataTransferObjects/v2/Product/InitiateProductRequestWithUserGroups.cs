using System.Collections.Generic;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product;

public class InitiateProductRequestWithUserGroups
{
        public InitiateProductRequest Product { get; set; }
        public IEnumerable<UserGroup> AllowedUserGroups { get; set; }
}