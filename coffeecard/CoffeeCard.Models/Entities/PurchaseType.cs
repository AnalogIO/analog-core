namespace CoffeeCard.Models.Entities
{
    public enum PurchaseType
    {
        /// Used for migration purposes. A purchase made using the mobilepay appswitch sdk
        MobilePayV1,

        /// A purchase made using the mobilepay webhookflow
        MobilePayV2,

        /// A purchase issued as a result of redeeming a voucher
        Voucher,

        /// A purchase issued by the user redeeming a product costing 0
        Free,

        /// Purchases performed in the cafe, for users without in-app payment options
        PointOfSale,
    }
}
