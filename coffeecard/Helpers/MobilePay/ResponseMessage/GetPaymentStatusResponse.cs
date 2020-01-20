namespace coffeecard.Helpers.MobilePay.ResponseMessage
{
    public class GetPaymentStatusResponse : IMobilePayAPIResponse
    {
        public PaymentStatus LatestPaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public double OrginalAmount { get; set; }
    }
}
