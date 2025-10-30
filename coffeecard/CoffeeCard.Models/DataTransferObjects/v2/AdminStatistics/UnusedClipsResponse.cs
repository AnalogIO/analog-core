namespace CoffeeCard.Models.DataTransferObjects.v2.AdminStatistics
{
    /// <summary>
    /// Initialize a response with unused clips data
    /// </summary>
    public class UnusedClipsResponse
    {
        /// <summary>
        /// The id of the product
        /// </summary>
        /// <value> Product Id </value>
        /// <example> 1 </example>
        public required int ProductId { get; set; }

        /// <summary>
        /// The name of the product
        /// </summary>
        /// <value> Product Name </value>
        /// <example> Americano </example>
        public required string ProductName { get; set; }

        /// <summary>
        /// The number of tickets unused in a purchase
        /// </summary>
        /// <value> Tickets left </value>
        /// <example> 8 </example>
        public required int TicketsLeft { get; set; }

        /// <summary>
        /// The value of the unused purchases of a given product
        /// </summary>
        /// <value> Value of the unused purchases </value>
        /// <example> 40.2 </example>
        public required decimal UnusedPurchasesValue { get; set; }
    }
}
