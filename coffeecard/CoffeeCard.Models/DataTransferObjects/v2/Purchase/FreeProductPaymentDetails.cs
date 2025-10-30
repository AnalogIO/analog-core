namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Payment details for a free purchase
    /// </summary>
    /// <example>
    /// {
    ///     "paymentType": "FreeProduct",
    ///     "orderId": "f5cb3e0f-3b9b-4f50-8c4f-a7450f300a5c"
    /// }
    /// </example>
    public class FreePurchasePaymentDetails : PaymentDetails
    {
        /// <summary>
        /// Creates a new instance of <see cref="FreePurchasePaymentDetails"/>
        /// </summary>
        public FreePurchasePaymentDetails(string orderId)
        {
            PaymentType = PaymentType.FreePurchase;
            OrderId = orderId;
        }
    }
}
