using System;
using System.Net.Http;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using Microsoft.Extensions.DependencyInjection;

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
        public static void AddMobilePayHttpClients(
            this IServiceCollection services,
            MobilePaySettings mobilePaySettings
        )
        {
            mobilePaySettings.Validate();
            services.AddTransient<MobilePayAuthorizationDelegatingHandler>();
            services.AddTransient<MobilePayIdempotencyDelegatingHandler>();

            services
                .AddHttpClient<IEPaymentClient, EPaymentClient>(client =>
                {
                    client.AddDefaultHeaders(mobilePaySettings);
                    client.BaseAddress = mobilePaySettings.ApiUrl;
                })
                .AddHttpMessageHandler<MobilePayAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<MobilePayIdempotencyDelegatingHandler>();

            services
                .AddHttpClient<IWebhooksClient, WebhooksClient>(client =>
                {
                    client.AddDefaultHeaders(mobilePaySettings);
                    client.BaseAddress = mobilePaySettings.ApiUrl;
                })
                .AddHttpMessageHandler<MobilePayAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAccessTokenClient, AccessTokenClient>(client =>
            {
                client.AddDefaultHeaders(mobilePaySettings);
                client.BaseAddress = mobilePaySettings.ApiUrl;
            });
        }

        private static void AddDefaultHeaders(
            this HttpClient httpClient,
            MobilePaySettings settings
        )
        {
            httpClient.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key",
                settings.OcpApimSubscriptionKey
            );
            httpClient.DefaultRequestHeaders.Add(
                "Merchant-Serial-Number",
                settings.MerchantSerialNumber
            );
        }
    }
}
