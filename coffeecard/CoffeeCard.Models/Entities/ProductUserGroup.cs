namespace CoffeeCard.Models.Entities
{
    public class ProductUserGroup
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public UserGroup UserGroup { get; set; }
    }
}
