using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Clients
{
    public class AccessTokenClientTest : BaseClientTest
    {
        private readonly Mock<ILogger<AccessTokenClient>> _loggerMock = new();

        [Fact(DisplayName = "GetToken returns valid response on success")]
        public async Task GetToken_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var expectedResponse = new AuthorizationTokenResponse
            {
                Access_token = "test-access-token",
                Expires_in = "3600",
                Token_type = "Bearer",
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new AccessTokenClient(httpClient, _loggerMock.Object);

            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";

            // Act
            var result = await client.GetToken(clientId, clientSecret);

            // Assert
            Assert.Equal(expectedResponse.Access_token, result.Access_token);
            Assert.Equal(expectedResponse.Expires_in, result.Expires_in);
            Assert.Equal(expectedResponse.Token_type, result.Token_type);
        }

        [Fact(DisplayName = "GetToken throws exception on error response")]
        public async Task GetToken_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var errorResponse = new
            {
                error = "invalid_client",
                error_description = "Invalid client credentials",
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.Unauthorized, errorResponse);
            var client = new AccessTokenClient(httpClient, _loggerMock.Object);

            var clientId = "invalid-client-id";
            var clientSecret = "invalid-client-secret";

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.GetToken(clientId, clientSecret)
            );

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        private void VerifyLoggingOccurred()
        {
            _loggerMock.Verify(
                x =>
                    x.Log(
                        It.Is<LogLevel>(l => l == LogLevel.Error),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!
                    ),
                Times.Once
            );
        }
    }
}
