namespace CoffeeCard.Models.DataTransferObjects.MobilePay
{
    public class MobilePayPaymentStatusDTO
    {
        public string LatestPaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public float OriginalAmount { get; set; }
    }
}