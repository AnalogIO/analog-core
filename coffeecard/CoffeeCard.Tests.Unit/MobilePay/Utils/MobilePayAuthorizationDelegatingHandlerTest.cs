using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.MobilePay.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Moq;
using Moq.Protected;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Utils
{
    public class MobilePayAuthorizationDelegatingHandlerTest
    {
        private readonly Mock<IMobilePayAccessTokenService> _accessTokenServiceMock = new();
        private readonly Mock<HttpMessageHandler> _innerHandlerMock = new();

        [Fact(DisplayName = "SendAsync adds authorization header with token")]
        public async Task SendAsync_AddsAuthorizationHeader_WithToken()
        {
            // Arrange
            var accessToken = "test-access-token";
            var accessTokenResponse = new GetAuthorizationTokenResponse
            {
                AccessToken = accessToken,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            };

            _accessTokenServiceMock
                .Setup(x => x.GetAuthorizationTokenAsync())
                .ReturnsAsync(accessTokenResponse);

            // Set up inner handler to return success response
            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var handler = new MobilePayAuthorizationDelegatingHandler(
                _accessTokenServiceMock.Object
            )
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com");

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            _accessTokenServiceMock.Verify(x => x.GetAuthorizationTokenAsync(), Times.Once);

            _innerHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Headers.Authorization != null
                        && req.Headers.Authorization.Scheme
                            == JwtBearerDefaults.AuthenticationScheme
                        && req.Headers.Authorization.Parameter == accessToken
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact(DisplayName = "SendAsync returns response from inner handler")]
        public async Task SendAsync_ReturnsResponse_FromInnerHandler()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.Created;
            var expectedContent = "test response content";

            var accessTokenResponse = new GetAuthorizationTokenResponse
            {
                AccessToken = "any-token",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            };

            _accessTokenServiceMock
                .Setup(x => x.GetAuthorizationTokenAsync())
                .ReturnsAsync(accessTokenResponse);

            // Set up inner handler to return specific response
            var expectedResponse = new HttpResponseMessage(expectedStatusCode)
            {
                Content = new StringContent(expectedContent),
            };

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            var handler = new MobilePayAuthorizationDelegatingHandler(
                _accessTokenServiceMock.Object
            )
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedContent, actualContent);
        }

        [Fact(DisplayName = "SendAsync handles token service exceptions")]
        public async Task SendAsync_HandlesTokenServiceExceptions()
        {
            // Arrange
            _accessTokenServiceMock
                .Setup(x => x.GetAuthorizationTokenAsync())
                .ThrowsAsync(new InvalidOperationException("Token service failed"));

            var handler = new MobilePayAuthorizationDelegatingHandler(
                _accessTokenServiceMock.Object
            )
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                invoker.SendAsync(request, CancellationToken.None)
            );

            // Verify the inner handler was never called
            _innerHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact(DisplayName = "SendAsync preserves original request headers")]
        public async Task SendAsync_PreservesOriginalRequestHeaders()
        {
            // Arrange
            var accessTokenResponse = new GetAuthorizationTokenResponse
            {
                AccessToken = "test-token",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            };

            _accessTokenServiceMock
                .Setup(x => x.GetAuthorizationTokenAsync())
                .ReturnsAsync(accessTokenResponse);

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var handler = new MobilePayAuthorizationDelegatingHandler(
                _accessTokenServiceMock.Object
            )
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com");

            // Add custom headers to the request
            request.Headers.Add("X-Custom-Header", "custom-value");
            request.Headers.Add("Accept", "application/json");

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            _innerHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Headers.Contains("X-Custom-Header")
                        && req.Headers.GetValues("X-Custom-Header").First() == "custom-value"
                        && req.Headers.Accept.First().MediaType == "application/json"
                        && req.Headers.Authorization != null
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact(DisplayName = "SendAsync correctly handles cancellation token")]
        public async Task SendAsync_CorrectlyHandlesCancellationToken()
        {
            // Arrange
            var accessTokenResponse = new GetAuthorizationTokenResponse
            {
                AccessToken = "test-token",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
            };

            _accessTokenServiceMock
                .Setup(x => x.GetAuthorizationTokenAsync())
                .ReturnsAsync(accessTokenResponse);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var handler = new MobilePayAuthorizationDelegatingHandler(
                _accessTokenServiceMock.Object
            )
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com");

            // Act
            await invoker.SendAsync(request, cancellationToken);

            // Assert - verify cancellation token is passed through
            _innerHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.Is<CancellationToken>(ct => ct.Equals(cancellationToken))
                );
        }
    }
}
