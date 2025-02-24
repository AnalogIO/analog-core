using System;
using System.Net.Http.Headers;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
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
        public static void AddMobilePayHttpClients(this IServiceCollection services,
            MobilePaySettingsV2 mobilePaySettings)
        {
            mobilePaySettings.Validate();

            services.AddTransient<MobilePayAuthorizationDelegatingHandler>();

            var apiKeyAuthentication = new AuthenticationHeaderValue("Bearer", mobilePaySettings.ApiKey);

            services.AddHttpClient<ePaymentClient>(client =>
            {
                client.BaseAddress = new Uri(mobilePaySettings.ApiUrl + "epayment/");
            }).AddHttpMessageHandler<MobilePayAuthorizationDelegatingHandler>();

            services.AddHttpClient<WebhooksClient>(client =>
            {
                client.BaseAddress = mobilePaySettings.ApiUrl;
                client.DefaultRequestHeaders.Authorization = apiKeyAuthentication;
            });

            services.AddHttpClient<AccessTokenClient>(client =>
            {
                client.BaseAddress = mobilePaySettings.ApiUrl;
            });
        }
    }
}
