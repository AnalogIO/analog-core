using System.Collections.Generic;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.Product;

public class AddProductRequestWithUserGroups
{
        public AddProductRequest Product { get; set; }
        public IEnumerable<UserGroup> AllowedUserGroups { get; set; }
}