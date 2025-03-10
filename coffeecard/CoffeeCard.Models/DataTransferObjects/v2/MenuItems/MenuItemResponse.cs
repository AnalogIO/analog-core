using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.MenuItems
{
    /// <summary>
    /// Represents a menu item that can be redeemed with a ticket
    /// </summary>
    public class MenuItemResponse
    {
        /// <summary>
        /// Id of menu item
        /// </summary>
        /// <value>Menu item Id</value>
        /// <example>1</example>
        [Required]
        public required int Id { get; set; }

        /// <summary>
        /// Name of menu item
        /// </summary>
        /// <value>Menu item name</value>
        /// <example>Cappuccino</example>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Whether or not this menu item is active
        /// </summary>
        /// <value>Menu item active</value>
        /// <example>true</example>
        [Required]
        public required bool Active { get; set; }
    }
}
