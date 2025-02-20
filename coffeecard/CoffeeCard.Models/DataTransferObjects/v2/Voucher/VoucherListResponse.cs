using System.Collections.Generic;

namespace CoffeeCard.Models.DataTransferObjects.v2.Voucher;

public class VoucherListResponse
{
    public int TotalVouchers { get; set; }
    
    public IEnumerable<VoucherResponse> Vouchers { get; set; }
}