namespace CoffeeCard.Models.DataTransferObjects.v2.Voucher
{
    /// <summary>
    /// 
    /// </summary>
    public class IssueVoucherRequest
    {
        /// <summary>
        /// Id of the product to create voucher codes for
        /// </summary>
        /// <example>
        /// {
        ///     "productId": "6"
        /// }
        /// </example>
        public int ProductId { get; set; }
        
        /// <summary>
        /// The amount of vouchers to be issued
        /// </summary>
        /// <example>
        /// {
        ///     "amount": "10"
        /// }
        /// </example>
        public int Amount { get; set; }
    }
}