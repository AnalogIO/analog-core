using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Voucher
{
    /// <summary>
    /// Request object for creating voucher codes
    /// </summary>
    /// <example>
    /// {
    ///     "ProductId": 6,
    ///     "Amount": 10,
    ///     "VoucherPrefix": "ABC",
    ///     "Description": "Voucher codes for intro week",
    ///     "Requester": "John Doe"
    /// }
    /// </example>
    public class IssueVoucherRequest
    {
        /// <summary>
        /// Id of the product to create voucher codes for
        /// </summary>
        /// <example>6</example>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// The amount of vouchers to be issued
        /// </summary>
        /// <example>10</example>
        [Required]
        public int Amount { get; set; }

        /// <summary>
        /// A 3 character long user defined prefix for every voucher code
        /// </summary>
        /// <example>ABC</example>
        [StringLength(3, MinimumLength = 3)]
        [Required]
        public string VoucherPrefix { get; set; }

        /// <summary>
        /// Description of the purpose for the creation of vouchers
        /// </summary>
        /// <example>Voucher codes for intro week   </example>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// The requester of the voucher codes
        /// </summary>
        /// <example>John Doe</example>
        [Required]
        public string Requester { get; set; }
    }
}
