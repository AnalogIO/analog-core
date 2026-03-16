using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoffeeCard.Models.DataTransferObjects.v2.MenuItems;

namespace CoffeeCard.Models.DataTransferObjects.v2.CoffeeCards
{
    /// <summary>
    /// A coffee card is a purchasable product and its remaining unused tickets for a user.
    /// </summary>
    /// <example>
    /// {
    ///     "productId": 1,
    ///     "productName": "Filter Coffee",
    ///     "ticketsLeft": 5,
    ///     "price": 50,
    ///     "eligibleMenuItems": [
    ///         { "id": 1, "name": "Cappuccino" },
    ///         { "id": 2, "name": "Caffe Latte" }
    ///     ]
    /// }
    /// </example>
    public class CoffeeCardResponse
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
        /// Price of product
        /// </summary>
        /// <value>Product Price</value>
        /// <example>50</example>
        [Required]
        public required int Price { get; set; }

        /// <summary>
        /// The menu items that this coffee card can be used on.
        /// </summary>
        /// <value>Menu items</value>
        /// <example>["Cappuccino", "Caffe Latte"]</example>
        [Required]
        public required IEnumerable<MenuItemResponse> EligibleMenuItems { get; set; }
    }
}