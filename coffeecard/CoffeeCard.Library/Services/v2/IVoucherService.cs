using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Voucher;

namespace CoffeeCard.Library.Services.v2
{
    public interface IVoucherService
    {
        /// <summary>
        /// Create new voucher codes
        /// </summary>
        /// <param name="request">Request object with information on how many codes to create and for which product</param>
        /// <returns>List of response objects containing voucher codes and product information</returns>
        Task<IEnumerable<IssueVoucherResponse>> CreateVouchers(IssueVoucherRequest request);
    }
}
