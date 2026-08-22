using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2.PaymentStrategies
{
    /// <summary>
    /// Result of initiating a payment via a <see cref="IPaymentStrategy"/>.
    /// </summary>
    public sealed record PaymentInitiationResult(
        PurchaseStatus PurchaseStatus,
        string? TransactionId,
        PaymentDetails PaymentDetails
    );
}

