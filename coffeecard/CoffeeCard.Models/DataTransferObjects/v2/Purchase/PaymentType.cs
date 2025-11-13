using System;
using CoffeeCard.Models.Entities;

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
        FreePurchase,
    }

    /// <summary>
    /// Extension methods for the paymentType
    /// </summary>
    public static class PaymentTypeExtension
    {
        /// Converts the payment type to a corresponding PurchasType
        public static PurchaseType ToPurchaseType(this PaymentType paymentType)
        {
            return paymentType switch
            {
                PaymentType.MobilePay => PurchaseType.MobilePayV2,
                PaymentType.FreePurchase => PurchaseType.Free,
                _ => throw new ArgumentException("Unknown enum given to PaymentTypeExtension"),
            };
        }
    }
}
