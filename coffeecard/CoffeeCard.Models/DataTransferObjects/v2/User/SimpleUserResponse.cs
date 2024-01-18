using CoffeeCard.Models.Entities;

namespace CoffeeCard.Models.DataTransferObjects.v2.User
{
    public class SimpleUserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserGroup UserGroup { get; set; }
        public UserState State { get; set; }
    }
}