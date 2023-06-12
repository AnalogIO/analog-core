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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductUserGroup"/> class with the specified product and user group.
        /// </summary>
        /// <param name="product">The product associated with this product user group.</param>
        /// <param name="userGroup">The user group associated with this product user group.</param>
        public ProductUserGroup(Product product, UserGroup userGroup)
        {
            ProductId = product.Id;
            Product = product;
            UserGroup = userGroup;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductUserGroup"/> class.
        /// </summary>
        public ProductUserGroup()
        {
        }
    }
}