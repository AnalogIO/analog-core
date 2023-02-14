namespace CoffeeCard.MobilePay.ResponseMessage
{
    public sealed class CaptureAmountResponse : IMobilePayApiResponse
    {
        public string TransactionId { get; set; }
    }
}