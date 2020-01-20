namespace coffeecard.Helpers.MobilePay.ErrorMessage
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
        Other
    }
}