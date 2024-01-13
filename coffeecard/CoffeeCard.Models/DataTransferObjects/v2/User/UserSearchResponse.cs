using System.Collections.Generic;

namespace CoffeeCard.Models.DataTransferObjects.v2.User;

public class UserSearchResponse
{
    public int TotalUsers { get; set; }
    public IEnumerable<SimpleUserResponse> Users;
}