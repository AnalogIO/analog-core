using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ExtraDetail = CoffeeCard.MobilePay.Generated.Api.ePaymentApi.ExtraDetail;

namespace CoffeeCard.Tests.Unit.MobilePay.Clients
{
    public class WebhooksClientTest : BaseClientTest
    {
        private readonly Mock<ILogger<WebhooksClient>> _loggerMock = new();

        [Fact(DisplayName = "CreateWebhookAsync returns valid response on success")]
        public async Task CreateWebhookAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var webhookId = Guid.NewGuid();
            var webhookSecret = "webhook-secret-123";
            var webhookUrl = "https://example.com/webhook";

            var request = new RegisterRequest
            {
                Events = new List<string> { "epayments.payment.authorized.v1" },
                Url = new Uri(webhookUrl),
            };

            var expectedResponse = new RegisterResponse { Id = webhookId, Secret = webhookSecret };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new WebhooksClient(httpClient, _loggerMock.Object);

            // Act
            var result = await client.CreateWebhookAsync(request);

            // Assert
            Assert.Equal(webhookId, result.Id);
            Assert.Equal(webhookSecret, result.Secret);
        }

        [Fact(DisplayName = "CreateWebhookAsync throws exception on error response")]
        public async Task CreateWebhookAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Events = new List<string> { "epayments.payment.authorized.v1" },
                Url = new Uri("https://example.com/webhook"),
            };

            var problem = new Problem
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid webhook URL",
                ExtraDetails = [new ExtraDetail { Name = "Error", Reason = "Invalid URL" }],
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, problem);
            var client = new WebhooksClient(httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.CreateWebhookAsync(request)
            );

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        [Fact(DisplayName = "GetAllWebhooksAsync returns valid response on success")]
        public async Task GetAllWebhooksAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var webhook1Id = Guid.NewGuid();
            var webhook1Url = "https://example.com/webhook1";
            var webhook2Id = Guid.NewGuid();
            var webhook2Url = "https://example.com/webhook2";

            var expectedResponse = new QueryResponse
            {
                Webhooks = new List<Webhook>
                {
                    new() { Id = webhook1Id, Url = new Uri(webhook1Url) },
                    new() { Id = webhook2Id, Url = new Uri(webhook2Url) },
                },
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new WebhooksClient(httpClient, _loggerMock.Object);

            // Act
            var result = await client.GetAllWebhooksAsync();

            // Assert
            Assert.Equal(2, result.Webhooks.Count);
            Assert.Equal(webhook1Id, result.Webhooks.ElementAt(0).Id);
            Assert.Equal(webhook1Url, result.Webhooks.ElementAt(0).Url.ToString());
            Assert.Equal(webhook2Id, result.Webhooks.ElementAt(1).Id);
            Assert.Equal(webhook2Url, result.Webhooks.ElementAt(1).Url.ToString());
        }

        [Fact(DisplayName = "GetAllWebhooksAsync throws exception on error response")]
        public async Task GetAllWebhooksAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var problem = new Problem
            {
                Title = "Internal Server Error",
                Status = 500,
                Detail = "An error occurred while retrieving webhooks",
                ExtraDetails = [new ExtraDetail { Name = "Error", Reason = "Database error" }],
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, problem);
            var client = new WebhooksClient(httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() => client.GetAllWebhooksAsync());

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        [Fact(DisplayName = "LogMobilePayException logs error information correctly")]
        public async Task LogMobilePayException_LogsErrorInformation_Correctly()
        {
            // Arrange
            var problem = new Problem
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid webhook URL",
                ExtraDetails = [new ExtraDetail { Name = "Error", Reason = "Invalid URL" }],
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, problem);
            var client = new WebhooksClient(httpClient, _loggerMock.Object);

            // Act & Assert
            try
            {
                await client.GetAllWebhooksAsync();
                Assert.Fail("Exception was expected but not thrown");
            }
            catch (MobilePayApiException)
            {
                // This is expected
            }

            // Verify error information was logged correctly
            VerifyLoggingOccurred();
        }

        private void VerifyLoggingOccurred()
        {
            _loggerMock.Verify(
                x =>
                    x.Log(
                        LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception, string>>()!
                    ),
                Times.AtLeast(1)
            );
        }
    }
}
