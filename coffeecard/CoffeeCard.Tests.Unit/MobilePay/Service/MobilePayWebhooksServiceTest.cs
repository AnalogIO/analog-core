using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Service
{
    public class MobilePayWebhooksServiceTest
    {
        private readonly Mock<IWebhooksClient> _webhooksClientMock = new();
        private readonly Mock<ILogger<MobilePayWebhooksService>> _loggerMock = new();

        [Fact(DisplayName = "RegisterWebhook creates webhook and returns response")]
        public async Task RegisterWebhook_CreatesWebhook_ReturnsResponse()
        {
            // Arrange
            var service = new MobilePayWebhooksService(
                _webhooksClientMock.Object,
                _loggerMock.Object
            );

            var webhookUrl = "https://example.com/webhook";
            var webhookId = Guid.NewGuid();
            var webhookSecret = "webhook-secret-123";

            var expectedResponse = new RegisterResponse { Id = webhookId, Secret = webhookSecret };

            _webhooksClientMock
                .Setup(x => x.CreateWebhookAsync(It.IsAny<RegisterRequest>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.RegisterWebhook(webhookUrl);

            // Assert
            Assert.Equal(webhookId, result.WebhookId);
            Assert.Equal(webhookUrl, result.Url.ToString());
            Assert.Equal(webhookSecret, result.Secret);

            // Verify client was called with correct events
            _webhooksClientMock.Verify(
                x =>
                    x.CreateWebhookAsync(
                        It.Is<RegisterRequest>(req =>
                            req.Url.ToString() == webhookUrl
                            && req.Events.Count == 4
                            && req.Events.Contains("epayments.payment.authorized.v1")
                            && req.Events.Contains("epayments.payment.cancelled.v1")
                            && req.Events.Contains("epayments.payment.expired.v1")
                            && req.Events.Contains("epayments.payment.aborted.v1")
                        )
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "GetWebhook returns webhook when it exists")]
        public async Task GetWebhook_ReturnsWebhook_WhenItExists()
        {
            // Arrange
            var service = new MobilePayWebhooksService(
                _webhooksClientMock.Object,
                _loggerMock.Object
            );

            var webhookId = Guid.NewGuid();
            var webhookUrl = "https://example.com/webhook";

            var allWebhooks = new QueryResponse
            {
                Webhooks = new List<Webhook>
                {
                    new Webhook { Id = webhookId, Url = new Uri(webhookUrl) },
                    new Webhook
                    {
                        Id = Guid.NewGuid(),
                        Url = new Uri("https://other-url.com/webhook"),
                    },
                },
            };

            _webhooksClientMock.Setup(x => x.GetAllWebhooksAsync()).ReturnsAsync(allWebhooks);

            // Act
            var result = await service.GetWebhook(webhookId);

            // Assert
            Assert.Equal(webhookId, result.WebhookId);
            Assert.Equal(webhookUrl, result.Url.ToString());

            // Verify client was called
            _webhooksClientMock.Verify(x => x.GetAllWebhooksAsync(), Times.Once);
        }

        [Fact(DisplayName = "GetWebhook throws EntityNotFoundException when webhook doesn't exist")]
        public async Task GetWebhook_ThrowsEntityNotFoundException_WhenWebhookDoesntExist()
        {
            // Arrange
            var service = new MobilePayWebhooksService(
                _webhooksClientMock.Object,
                _loggerMock.Object
            );

            var webhookId = Guid.NewGuid();

            var allWebhooks = new QueryResponse
            {
                Webhooks = new List<Webhook>
                {
                    new Webhook
                    {
                        Id = Guid.NewGuid(),
                        Url = new Uri("https://other-url.com/webhook"),
                    },
                },
            };

            _webhooksClientMock.Setup(x => x.GetAllWebhooksAsync()).ReturnsAsync(allWebhooks);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                service.GetWebhook(webhookId)
            );

            Assert.Contains(webhookId.ToString(), exception.Message);

            // Verify client was called
            _webhooksClientMock.Verify(x => x.GetAllWebhooksAsync(), Times.Once);

            // Verify error was logged
            _loggerMock.Verify(
                x =>
                    x.Log(
                        LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception, string>>()!
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "GetAllWebhooks returns all webhooks")]
        public async Task GetAllWebhooks_ReturnsAllWebhooks()
        {
            // Arrange
            var service = new MobilePayWebhooksService(
                _webhooksClientMock.Object,
                _loggerMock.Object
            );

            var webhook1Id = Guid.NewGuid();
            var webhook1Url = "https://example.com/webhook1";
            var webhook2Id = Guid.NewGuid();
            var webhook2Url = "https://example.com/webhook2";

            var allWebhooks = new QueryResponse
            {
                Webhooks = new List<Webhook>
                {
                    new Webhook { Id = webhook1Id, Url = new Uri(webhook1Url) },
                    new Webhook { Id = webhook2Id, Url = new Uri(webhook2Url) },
                },
            };

            _webhooksClientMock.Setup(x => x.GetAllWebhooksAsync()).ReturnsAsync(allWebhooks);

            // Act
            var result = await service.GetAllWebhooks();

            // Assert
            Assert.Equal(2, result.Webhooks.Count);

            var webhook1 = result.Webhooks.First(w => w.WebhookId == webhook1Id);
            Assert.Equal(webhook1Url, webhook1.Url.ToString());

            var webhook2 = result.Webhooks.First(w => w.WebhookId == webhook2Id);
            Assert.Equal(webhook2Url, webhook2.Url.ToString());

            // Verify client was called
            _webhooksClientMock.Verify(x => x.GetAllWebhooksAsync(), Times.Once);
        }

        [Fact(DisplayName = "GetAllWebhooks returns empty list when no webhooks exist")]
        public async Task GetAllWebhooks_ReturnsEmptyList_WhenNoWebhooksExist()
        {
            // Arrange
            var service = new MobilePayWebhooksService(
                _webhooksClientMock.Object,
                _loggerMock.Object
            );

            var allWebhooks = new QueryResponse { Webhooks = new List<Webhook>() };

            _webhooksClientMock.Setup(x => x.GetAllWebhooksAsync()).ReturnsAsync(allWebhooks);

            // Act
            var result = await service.GetAllWebhooks();

            // Assert
            Assert.Empty(result.Webhooks);

            // Verify client was called
            _webhooksClientMock.Verify(x => x.GetAllWebhooksAsync(), Times.Once);
        }
    }
}
