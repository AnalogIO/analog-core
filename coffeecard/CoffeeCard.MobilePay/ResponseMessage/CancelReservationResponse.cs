namespace CoffeeCard.MobilePay.ResponseMessage
{
    public sealed class CancelReservationResponse : IMobilePayApiResponse
    {
        public string TransactionId { get; set; }
    }
}