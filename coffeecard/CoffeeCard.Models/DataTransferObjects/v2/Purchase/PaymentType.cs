namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// PaymentType represents the type of Payment which is used to fulfill a purchase
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// MobilePay App Payments
        /// </summary>
        MobilePay,

        /// <summary>
        /// Free purchase
        /// </summary>
        FreePurchase
    }
}