using System.ComponentModel.DataAnnotations;

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
        /// <example>6</example>
        public int ProductId { get; set; }
        
        /// <summary>
        /// The amount of vouchers to be issued
        /// </summary>
        /// <example>10</example>
        public int Amount { get; set; }

        /// <summary>
        /// A 3 character long user defined prefix for every voucher code
        /// </summary>
        /// <example>ABC</example>
        [StringLength(3, MinimumLength = 3)]
        public string VoucherPrefix { get; set; }

        /// <summary>
        /// Short description of the voucher code creation
        /// </summary>
        /// <example>"Voucher codes for intro week"</example>
        public string Description { get; set; }

        /// <summary>
        /// The requester of the voucher codes
        /// </summary>
        /// <example>"John Doe"</example>
        public string Requester { get; set; }
    }
}