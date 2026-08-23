using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Services.v2.PaymentStrategies;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCardApi.Integrations.Nexi.Generated.Client;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCardApi.Integrations.Nexi;

public static class NexiServiceCollectionExtension
{
    public static IServiceCollection RegisterNexiModule(this IServiceCollection services)
    {
        services.AddTransient<NexiAuthDelegatingHandler>();
        services
            .AddHttpClient<NexiClient>(
                (sp, client) =>
                {
                    var nexiSettings = sp.GetRequiredService<NexiSettings>();
                    client.BaseAddress = nexiSettings.ApiUrl;
                }
            )
            .AddHttpMessageHandler<NexiAuthDelegatingHandler>();
        services.AddKeyedScoped<IPaymentStrategy, NexiStrategy>(PaymentType.Nexi);

        return services;
    }
}
