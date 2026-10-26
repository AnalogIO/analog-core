using CoffeeCard.Models.DataTransferObjects.v2.Purchase;

namespace CoffeeCard.Library.Services.v2.PaymentStrategies
{
    /// <summary>
    /// Factory that resolves the correct <see cref="IPaymentStrategy"/> for a given <see cref="PaymentType"/>.
    /// </summary>
    public interface IPaymentStrategyFactory
    {
        /// <summary>
        /// Returns the <see cref="IPaymentStrategy"/> registered for <paramref name="paymentType"/>.
        /// </summary>
        IPaymentStrategy GetStrategy(PaymentType paymentType);
    }
}
