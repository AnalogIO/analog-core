using System;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCard.Library.Services.v2.PaymentStrategies
{
    /// <summary>
    /// Resolves <see cref="IPaymentStrategy"/> instances via keyed DI services.
    /// The key used on both registration and resolution sides is a <see cref="PaymentType"/> enum value,
    /// which keeps the mapping compile-time safe — no magic strings on either side.
    /// </summary>
    public sealed class PaymentStrategyFactory : IPaymentStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public IPaymentStrategy GetStrategy(PaymentType paymentType) =>
            _serviceProvider.GetRequiredKeyedService<IPaymentStrategy>(paymentType);
    }
}
