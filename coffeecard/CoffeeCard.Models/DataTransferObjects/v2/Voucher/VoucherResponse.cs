namespace CoffeeCard.Models.DataTransferObjects.v2.Voucher
{
    public class VoucherResponse
    {
        public int Id { get; init; }

        public string Code { get; init; }

        public string Description { get; init; }

        public string Requester { get; init; }

        public VoucherStatus Status { get; init; }
        
        public string ProductName { get; init; }
        
        public string UsedBy { get; init; }
    }
}