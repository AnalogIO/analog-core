namespace CoffeeCard.MobilePay.ResponseMessage
{
    public sealed class CaptureAmountResponse : IMobilePayAPIResponse
    {
        public string TransactionId { get; set; }
    }
}