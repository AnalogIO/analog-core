using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Products
{
    /// <summary>
    /// Represents a menu item that can be redeemed with a ticket
    /// </summary>
    /// <example>
    /// {
    ///    "id": 1,
    ///    "name": "Cappuccino",
    /// }
    /// </example>
    public class MenuItemResponse
    {
        /// <summary>
        /// Id of menu item
        /// </summary>
        /// <value>Menu item Id</value>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Name of menu item
        /// </summary>
        /// <value>Menu item name</value>
        /// <example>Cappuccino</example>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Whether or not this menu item is active
        /// </summary>
        /// <value>Menu item active</value>
        /// <example>true</example>
        public bool Active { get; set; }
    }
}
