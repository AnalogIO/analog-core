using System;

namespace CoffeeCard.Models.DataTransferObjects.v2.Voucher
{
    /// <summary>
    /// 
    /// </summary>
    public class IssueVoucherResponse
    {
        /// <summary>
        /// Voucher code to be redeemed in the app
        /// </summary>
        public string VoucherCode { get; set; }

        /// <summary>
        /// The id of the product the voucher code is for
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The name of the product the voucher code is for
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The date the voucher code was created
        /// </summary>
        public DateTime IssuedAt { get; set; }
    }
}