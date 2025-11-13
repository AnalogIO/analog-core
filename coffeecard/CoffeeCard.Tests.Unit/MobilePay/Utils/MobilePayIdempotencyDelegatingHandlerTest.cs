using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Utils;
using Moq;
using Moq.Protected;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Utils
{
    public class MobilePayIdempotencyDelegatingHandlerTest
    {
        private readonly Mock<HttpMessageHandler> _innerHandlerMock = new();

        [Fact(DisplayName = "SendAsync adds idempotency key header")]
        public async Task SendAsync_AddsIdempotencyKeyHeader()
        {
            // Arrange
            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var handler = new MobilePayIdempotencyDelegatingHandler
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com");

            // Act
            await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            _innerHandlerMock
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Headers.Contains("Idempotency-Key")
                        && !string.IsNullOrEmpty(req.Headers.GetValues("Idempotency-Key").First())
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact(DisplayName = "SendAsync adds different idempotency keys for different requests")]
        public async Task SendAsync_AddsDifferentIdempotencyKeys_ForDifferentRequests()
        {
            // Arrange
            string? firstIdempotencyKey = null;
            string? secondIdempotencyKey = null;

            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Callback<HttpRequestMessage, CancellationToken>(
                    (req, _) =>
                    {
                        if (firstIdempotencyKey == null)
                        {
                            firstIdempotencyKey = req.Headers.GetValues("Idempotency-Key").First();
                        }
                        else
                        {
                            secondIdempotencyKey = req.Headers.GetValues("Idempotency-Key").First();
                        }
                    }
                );

            var handler = new MobilePayIdempotencyDelegatingHandler
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request1 = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com");
            var request2 = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com");

            // Act
            await invoker.SendAsync(request1, CancellationToken.None);
            await invoker.SendAsync(request2, CancellationToken.None);

            // Assert
            Assert.NotNull(firstIdempotencyKey);
            Assert.NotNull(secondIdempotencyKey);
            Assert.NotEqual(firstIdempotencyKey, secondIdempotencyKey);
        }

        [Fact(DisplayName = "SendAsync returns response from inner handler")]
        public async Task SendAsync_ReturnsResponse_FromInnerHandler()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.Created;
            var expectedContent = "test response content";

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

            var handler = new MobilePayIdempotencyDelegatingHandler
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com");

            // Act
            var response = await invoker.SendAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            var actualContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(expectedContent, actualContent);
        }

        [Fact(DisplayName = "SendAsync correctly passes cancellation token")]
        public async Task SendAsync_CorrectlyPassesCancellationToken()
        {
            // Arrange
            _innerHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var handler = new MobilePayIdempotencyDelegatingHandler
            {
                InnerHandler = _innerHandlerMock.Object,
            };

            var invoker = new HttpMessageInvoker(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com");
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            await invoker.SendAsync(request, cancellationToken);

            // Assert
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
