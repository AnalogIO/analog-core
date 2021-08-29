namespace CoffeeCard.MobilePay.ErrorMessage
{
    public enum MobilePayErrorReason
    {
        InvalidOrderId,
        InvalidMerchantId,
        PartialCaptureNotPossible,
        InvalidAmount,
        InvalidTestModeHeader,
        MerchantNotFound,
        OrderNotFound,
        TransactionAlreadyCaptured,
        TransactionAlreadyCancelled,
        ReservationNotFound,
        Failed,
        Other
    }
}