using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeCard.Models.Entities
{
    public class ProductUserGroup
    {
        [ForeignKey("Product")] public int ProductId { get; set; }

        public Product Product { get; set; }

        public UserGroup UserGroup { get; set; }
    }
}