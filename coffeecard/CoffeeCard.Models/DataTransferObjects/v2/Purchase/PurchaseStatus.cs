namespace CoffeeCard.Models.DataTransferObjects.v2.Purchase
{
    /// <summary>
    /// Status of purchase
    /// </summary>
    public enum PurchaseStatus
    {
        /// <summary>
        /// Purchase has been fulfilled and tickets delivered to use
        /// </summary>
        Completed,

        /// <summary>
        /// Purchase has been cancelled and no deduction has been made at Payment provider
        /// </summary>
        Cancelled,

        /// <summary>
        /// Purchase awaits fulfillment at the Payment Provider
        /// </summary>
        PendingPayment,

        /// <summary>
        /// Purchase has been cancelled and deduction has been refunded on user's account
        /// </summary>
        Refunded,

        /// <summary>
        /// Purchase expired due to no completion by the user within the required time
        /// </summary>
        Expired,
    }
}
