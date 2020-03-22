namespace CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay
{
    public class MobilePayPaymentStatusDto
    {
        public string LatestPaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public float OriginalAmount { get; set; }
    }
}