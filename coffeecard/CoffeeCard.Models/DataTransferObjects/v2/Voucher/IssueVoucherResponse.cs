using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Voucher
{
    /// <summary>
    /// Response object for creating voucher codes
    /// </summary>
    /// <example>
    /// {
    ///     "VoucherCode": "ABC-12345678",
    ///     "ProductId": 6,
    ///     "ProductName": "Coffee",
    ///     "IssuedAt": "2023-02-07T12:00:00"
    /// }
    /// </example>
    public class IssueVoucherResponse
    {
        /// <summary>
        /// Voucher code to be redeemed in the app
        /// </summary>
        [Required]
        public required string VoucherCode { get; set; }

        /// <summary>
        /// The id of the product the voucher code is for
        /// </summary>
        [Required]
        public required int ProductId { get; set; }

        /// <summary>
        /// The name of the product the voucher code is for
        /// </summary>
        [Required]
        public required string ProductName { get; set; }

        /// <summary>
        /// The date the voucher code was created
        /// </summary>
        [Required]
        public required DateTime IssuedAt { get; set; }
    }
}
