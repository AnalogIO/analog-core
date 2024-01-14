namespace CoffeeCard.MobilePay.RefundConsoleApp.Model
{
    public class CompletedOrder
    {
        public string OrderId { get; }

        public CompletedOrder(string orderId)
        {
            OrderId = orderId;
        }
    }
}
