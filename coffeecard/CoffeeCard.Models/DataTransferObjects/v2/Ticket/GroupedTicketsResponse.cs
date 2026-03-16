using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.DataTransferObjects.v2.MenuItems;

namespace CoffeeCard.Models.DataTransferObjects.v2.GroupedTicketsResponse
{
    /// <summary>
    /// A collection of unused tickets a user owns for a product.
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1,
    ///     "productName": "Filter Coffee",
    ///     "ticketsLeft": 5,
    ///     "eligibleMenuItems": [
    ///         { "id": 1, "name": "Cappuccino" },
    ///         { "id": 2, "name": "Caffe Latte" }
    ///     ]
    /// }
    /// </example>
    public class GroupedTicketsResponse
    {
        /// <summary>
        /// Id of product
        /// </summary>
        /// <value>Product Id</value>
        /// <example>1</example>
        [Required]
        public required int ProductId { get; set; }

        /// <summary>
        /// Name of product
        /// </summary>
        /// <value>Product Name</value>
        /// <example>Filter Coffee</example>
        [Required]
        public required string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Remaining (if any) unused tickets left for product
        /// </summary>
        /// <value>Remaining Tickets</value>
        /// <example>5</example>
        [Required]
        public required int TicketsLeft { get; set; }

        /// <summary>
        /// The menu items that this ticket can be used on.
        /// </summary>
        /// <value>Menu items</value>
        /// <example>[{ "id": 1, "name": "Cappuccino", "active": true }, { "id": 2, "name": "Caffe Latte", "active": false }]</example>
        [Required]
        public required IEnumerable<MenuItemResponse> EligibleMenuItems { get; set; }
    }
}
