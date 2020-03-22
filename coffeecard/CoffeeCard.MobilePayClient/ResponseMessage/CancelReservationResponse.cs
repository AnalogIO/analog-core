namespace CoffeeCard.MobilePay.ResponseMessage
{
    public sealed class CancelReservationResponse : IMobilePayAPIResponse
    {
        public string TransactionId { get; set; }
    }
}