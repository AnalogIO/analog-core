namespace CoffeeCard.MobilePay.RefundConsoleApp.Model
{
    public class RefundResponse
    {
        public RefundResponse(string orderId, Status status)
        {
            OrderId = orderId;
            Status = status;
        }

        public Status Status { get; }
        public string OrderId { get; }

        public string? OriginalTransactionId { get; set; }
        public string? RefundTransactionId { get; set; }
        public double? Remainder { get; set; }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(OrderId)}: {OrderId}, {nameof(OriginalTransactionId)}: {OriginalTransactionId}, {nameof(RefundTransactionId)}: {RefundTransactionId}, {nameof(Remainder)}: {Remainder}";
        }
    }

    public enum Status
    {
        Success,
        Failed,
    }
}
