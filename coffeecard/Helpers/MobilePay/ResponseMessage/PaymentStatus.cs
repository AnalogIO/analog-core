namespace coffeecard.Helpers.MobilePay.ResponseMessage
{
    public enum PaymentStatus
    {
        Reserved, // Payment is reserved.
        Cancelled, // Reservation is cancelled by merchant.
        Captured, // Payment is captured by merchant.
        TotalRefund, // Total payment is refunded by merchant.
        PartialRefund, // Payment is partially refunded by merchant.
        Rejected // The reservation, capture, refund or cancellation is rejected.
    }
}