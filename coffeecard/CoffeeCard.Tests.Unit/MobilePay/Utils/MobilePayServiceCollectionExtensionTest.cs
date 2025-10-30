using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.MobilePay.Utils;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Utils
{
    public class MobilePayServiceCollectionExtensionTest
    {
        private readonly MobilePaySettings _testSettings = new()
        {
            ApiUrl = new Uri("https://api.test.mobilepay.dk"),
            ClientId = Guid.NewGuid(),
            ClientSecret = "test-secret",
            OcpApimSubscriptionKey = "test-subscription-key",
            MerchantSerialNumber = "test-merchant-serial-number",
            WebhookUrl = "test-webhook-url",
            AnalogAppRedirectUri = "analogcoffeecard-test://notapp",
        };

        [Fact(DisplayName = "AddMobilePayHttpClients registers the delegating handlers")]
        public void AddMobilePayHttpClients_RegistersDelegatingHandlers()
        {
            // Arrange
            var services = new ServiceCollection();

            // Register required dependencies
            services.AddMemoryCache();
            services.AddSingleton(Mock.Of<IMobilePayAccessTokenService>());

            // Act
            services.AddMobilePayHttpClients(_testSettings);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var authHandler = serviceProvider.GetService<MobilePayAuthorizationDelegatingHandler>();
            var idempotencyHandler =
                serviceProvider.GetService<MobilePayIdempotencyDelegatingHandler>();

            Assert.NotNull(authHandler);
            Assert.NotNull(idempotencyHandler);
        }

        [Fact(DisplayName = "AddMobilePayHttpClients registers the HTTP clients")]
        public void AddMobilePayHttpClients_RegistersHttpClients()
        {
            // Arrange
            var services = new ServiceCollection();

            // Register required dependencies
            services.AddMemoryCache();
            services.AddSingleton(Mock.Of<IMobilePayAccessTokenService>());

            // Act
            services.AddMobilePayHttpClients(_testSettings);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            var ePaymentClient = serviceProvider.GetService<IEPaymentClient>();
            var webhooksClient = serviceProvider.GetService<IWebhooksClient>();
            var accessTokenClient = serviceProvider.GetService<IAccessTokenClient>();

            Assert.NotNull(ePaymentClient);
            Assert.NotNull(webhooksClient);
            Assert.NotNull(accessTokenClient);
        }

        [Fact(DisplayName = "AddMobilePayHttpClients throws when settings are invalid")]
        public void AddMobilePayHttpClients_ThrowsWhenSettingsAreInvalid()
        {
            // Arrange
            var services = new ServiceCollection();
            var invalidSettings = new MobilePaySettings(); // Empty settings

            // Act & Assert
            Assert.Throws<ValidationException>(() =>
                services.AddMobilePayHttpClients(invalidSettings)
            );
        }
    }
}
