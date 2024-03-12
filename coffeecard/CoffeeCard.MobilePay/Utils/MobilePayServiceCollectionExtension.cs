using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Generated.Api.PaymentsApi;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace CoffeeCard.MobilePay.Utils
{
    /// <summary>
    /// Extensions for setting up MobilePay services
    /// </summary>
    public static class MobilePayServiceCollectionExtension
    {
        /// <summary>
        /// Add and configure Http Client for MobilePay Apis
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="mobilePaySettings">Settings class with MobilePayApi configuration</param>
        public static void AddMobilePayHttpClients(this IServiceCollection services,
            MobilePaySettingsV2 mobilePaySettings)
        {
            mobilePaySettings.Validate();

            AuthenticationHeaderValue apiKeyAuthentication = new AuthenticationHeaderValue("Bearer", mobilePaySettings.ApiKey);

            _ = services.AddHttpClient<PaymentsApi>(client =>
            {
                client.BaseAddress = mobilePaySettings.ApiUrl;
                client.DefaultRequestHeaders.Authorization = apiKeyAuthentication;
            });

            _ = services.AddHttpClient<WebhooksApi>(client =>
            {
                client.BaseAddress = mobilePaySettings.ApiUrl;
                client.DefaultRequestHeaders.Authorization = apiKeyAuthentication;
            });
        }
    }
}