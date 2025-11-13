namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Represents a mapping between a product and a user group.
    /// </summary>
    public class ProductUserGroup
    {
        /// <summary>
        /// Gets or sets the ID of the product associated with this product user group.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product associated with this product user group.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the user group associated with this product user group.
        /// </summary>
        public UserGroup UserGroup { get; set; }
    }
}
