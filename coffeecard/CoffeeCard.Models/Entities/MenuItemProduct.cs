namespace CoffeeCard.Models.Entities
{
    /// <summary>
    /// Join table for Menu Item and Product eligibilities
    /// </summary>
    public class MenuItemProduct
    {
        /// <summary>
        /// Menu Item id
        /// </summary>
        public int MenuItemId { get; set; }

        /// <summary>
        /// Menu Item
        /// </summary>
        public MenuItem MenuItem { get; set; }

        /// <summary>
        /// Product Id
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Product
        /// </summary>
        public Product Product { get; set; }
    }
}
