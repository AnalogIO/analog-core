using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Voucher;

namespace CoffeeCard.Library.Services.v2
{
    public interface IVoucherService
    {
        Task<IEnumerable<IssueVoucherResponse>> CreateVouchers(IssueVoucherRequest request);
    }
}