using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Clients
{
    public class EPaymentClientTests : BaseClientTest
    {
        private readonly Mock<ILogger<EPaymentClient>> _loggerMock = new();

        [Fact(DisplayName = "CreatePaymentAsync returns valid response on success")]
        public async Task CreatePaymentAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var expectedResponse = new CreatePaymentResponse
            {
                Reference = "test-payment-123",
                RedirectUrl = new Uri("https://redirect.example.com"),
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new CreatePaymentRequest
            {
                Reference = "test-payment-123",
                Amount = new Amount { Currency = Currency.DKK, Value = 10000 },
                PaymentDescription = "Test payment",
            };

            // Act
            var result = await client.CreatePaymentAsync(request);

            // Assert
            Assert.Equal(expectedResponse.Reference, result.Reference);
            Assert.Equal(expectedResponse.RedirectUrl, result.RedirectUrl);
        }

        [Fact(DisplayName = "CreatePaymentAsync throws exception on error response")]
        public async Task CreatePaymentAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var problem = new Problem
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Invalid payment request",
                Type = new Uri("https://developer.mobilepay.dk/errors/bad-request"),
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, problem);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new CreatePaymentRequest
            {
                Reference = "invalid-reference",
                Amount = new Amount { Currency = Currency.DKK, Value = 10000 },
                PaymentDescription = "Test payment with error",
            };

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.CreatePaymentAsync(request)
            );

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        [Fact(DisplayName = "GetPaymentAsync returns valid response on success")]
        public async Task GetPaymentAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var expectedResponse = new GetPaymentResponse
            {
                Reference = paymentReference,
                State = State.CREATED,
                Amount = new Amount { Currency = Currency.DKK, Value = 10000 },
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            // Act
            var result = await client.GetPaymentAsync(paymentReference);

            // Assert
            Assert.Equal(expectedResponse.Reference, result.Reference);
            Assert.Equal(expectedResponse.State, result.State);
            Assert.Equal(expectedResponse.Amount.Currency, result.Amount.Currency);
            Assert.Equal(expectedResponse.Amount.Value, result.Amount.Value);
        }

        [Fact(DisplayName = "GetPaymentAsync throws exception on error response")]
        public async Task GetPaymentAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var paymentReference = "invalid-reference";
            var problem = new Problem
            {
                Title = "Not Found",
                Status = 404,
                Detail = "Payment not found",
                Type = new Uri("https://developer.mobilepay.dk/errors/not-found"),
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.NotFound, problem);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.GetPaymentAsync(paymentReference)
            );

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        [Fact(DisplayName = "RefundPaymentAsync returns valid response on success")]
        public async Task RefundPaymentAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var expectedResponse = new ModificationResponse
            {
                Reference = "refund-123",
                State = State.AUTHORIZED,
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new RefundModificationRequest
            {
                ModificationAmount = new Amount { Currency = Currency.DKK, Value = 5000 },
            };

            // Act
            var result = await client.RefundPaymentAsync(paymentReference, request);

            // Assert
            Assert.Equal(expectedResponse.Reference, result.Reference);
            Assert.Equal(expectedResponse.State, result.State);
        }

        [Fact(DisplayName = "RefundPaymentAsync throws exception on error response")]
        public async Task RefundPaymentAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var problem = new Problem
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Cannot refund more than the captured amount",
                Type = new Uri("https://developer.mobilepay.dk/errors/bad-request"),
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, problem);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new RefundModificationRequest
            {
                ModificationAmount = new Amount { Currency = Currency.DKK, Value = 20000 },
            };

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.RefundPaymentAsync(paymentReference, request)
            );

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        [Fact(DisplayName = "CapturePaymentAsync returns valid response on success")]
        public async Task CapturePaymentAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var expectedResponse = new ModificationResponse
            {
                Reference = "capture-123",
                State = State.AUTHORIZED,
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new CaptureModificationRequest
            {
                ModificationAmount = new Amount { Currency = Currency.DKK, Value = 10000 },
            };

            // Act
            var result = await client.CapturePaymentAsync(paymentReference, request);

            // Assert
            Assert.Equal(expectedResponse.Reference, result.Reference);
            Assert.Equal(expectedResponse.State, result.State);
        }

        [Fact(DisplayName = "CapturePaymentAsync throws exception on error response")]
        public async Task CapturePaymentAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var problem = new Problem
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Payment is not in a capturable state",
                Type = new Uri("https://developer.mobilepay.dk/errors/bad-request"),
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, problem);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new CaptureModificationRequest
            {
                ModificationAmount = new Amount { Currency = Currency.DKK, Value = 10000 },
            };

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.CapturePaymentAsync(paymentReference, request)
            );

            // Verify logging occurred
            VerifyLoggingOccurred();
        }

        [Fact(DisplayName = "CancelPaymentAsync returns valid response on success")]
        public async Task CancelPaymentAsync_ReturnsValidResponse_OnSuccess()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var expectedResponse = new ModificationResponse
            {
                Reference = "cancel-123",
                State = State.AUTHORIZED,
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, expectedResponse);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new CancelModificationRequest();

            // Act
            var result = await client.CancelPaymentAsync(paymentReference, request);

            // Assert
            Assert.Equal(expectedResponse.Reference, result.Reference);
            Assert.Equal(expectedResponse.State, result.State);
        }

        [Fact(DisplayName = "CancelPaymentAsync throws exception on error response")]
        public async Task CancelPaymentAsync_ThrowsException_OnErrorResponse()
        {
            // Arrange
            var paymentReference = "test-payment-123";
            var problem = new Problem
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Payment is not in a cancelable state",
                Type = new Uri("https://developer.mobilepay.dk/errors/bad-request"),
            };

            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, problem);
            var client = new EPaymentClient(httpClient, _loggerMock.Object);

            var request = new CancelModificationRequest();

            // Act & Assert
            await Assert.ThrowsAsync<MobilePayApiException>(() =>
                client.CancelPaymentAsync(paymentReference, request)
            );

            // Verify logging occurred
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
