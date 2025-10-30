using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.MobilePay.Service.v2;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.MobilePay.Service
{
    public class MobilePayPaymentsServiceTest
    {
        private readonly Mock<IEPaymentClient> _ePaymentClientMock = new();
        private readonly Mock<ILogger<MobilePayPaymentsService>> _loggerMock = new();
        private readonly MobilePaySettings _mobilePaySettings = new()
        {
            AnalogAppRedirectUri = "analogcoffeecard-test://mobilepay_purchase",
        };

        [Fact(DisplayName = "InitiatePayment creates correct payment request")]
        public async Task InitiatePayment_CreatesCorrectPaymentRequest()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var paymentRequest = new MobilePayPaymentRequest
            {
                OrderId = Guid.NewGuid(),
                Amount = 50,
                Description = "Test payment",
            };

            var expectedResponse = new CreatePaymentResponse
            {
                Reference = paymentRequest.OrderId.ToString(),
                RedirectUrl = new Uri("https://notmobilepay.app/redirect"),
            };

            _ePaymentClientMock
                .Setup(x => x.CreatePaymentAsync(It.IsAny<CreatePaymentRequest>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.InitiatePayment(paymentRequest);

            // Assert
            Assert.Equal(paymentRequest.OrderId.ToString(), result.PaymentId);
            Assert.Equal(expectedResponse.RedirectUrl.ToString(), result.MobilePayAppRedirectUri);

            // Verify correct request was made
            _ePaymentClientMock.Verify(
                x =>
                    x.CreatePaymentAsync(
                        It.Is<CreatePaymentRequest>(req =>
                            req.Amount.Value == paymentRequest.Amount * 100
                            && req.Amount.Currency == Currency.DKK
                            && req.Reference == paymentRequest.OrderId.ToString()
                            && req.PaymentDescription == paymentRequest.Description
                            && req.ReturnUrl == _mobilePaySettings.AnalogAppRedirectUri
                            && req.UserFlow == CreatePaymentRequestUserFlow.WEB_REDIRECT
                            && req.PaymentMethod.Type == PaymentMethodType.WALLET
                        )
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "GetPayment returns payment details")]
        public async Task GetPayment_ReturnsPaymentDetails()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var paymentId = Guid.NewGuid();
            var expectedResponse = new GetPaymentResponse
            {
                Reference = paymentId.ToString(),
                RedirectUrl = new Uri("https://notmobilepay.app/redirect"),
            };

            _ePaymentClientMock
                .Setup(x => x.GetPaymentAsync(paymentId.ToString()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await service.GetPayment(paymentId);

            // Assert
            Assert.Equal(paymentId.ToString(), result.PaymentId);
            Assert.Equal(expectedResponse.RedirectUrl.ToString(), result.MobilePayAppRedirectUri);

            // Verify client was called
            _ePaymentClientMock.Verify(x => x.GetPaymentAsync(paymentId.ToString()), Times.Once);
        }

        [Fact(DisplayName = "RefundPayment calls API correctly")]
        public async Task RefundPayment_CallsApiCorrectly()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var purchase = new Purchase { ExternalTransactionId = "transaction-123" };

            var refundAmount = 25;

            // Act
            var result = await service.RefundPayment(purchase, refundAmount);

            // Assert
            Assert.True(result);

            // Verify correct refund request was made
            _ePaymentClientMock.Verify(
                x =>
                    x.RefundPaymentAsync(
                        purchase.ExternalTransactionId,
                        It.Is<RefundModificationRequest>(req =>
                            req.ModificationAmount.Currency == Currency.DKK
                            && req.ModificationAmount.Value == refundAmount
                        )
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "RefundPayment throws ArgumentNullException when purchase is null")]
        public async Task RefundPayment_ThrowsArgumentNullException_WhenPurchaseIsNull()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.RefundPayment(null!, 25));
        }

        [Fact(
            DisplayName = "RefundPayment throws ArgumentNullException when ExternalTransactionId is null"
        )]
        public async Task RefundPayment_ThrowsArgumentNullException_WhenExternalTransactionIdIsNull()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var purchase = new Purchase { ExternalTransactionId = null };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.RefundPayment(purchase, 25)
            );
        }

        [Fact(DisplayName = "CapturePayment calls API correctly")]
        public async Task CapturePayment_CallsApiCorrectly()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var paymentId = Guid.NewGuid();
            var captureAmount = 50;

            // Act
            await service.CapturePayment(paymentId, captureAmount);

            // Assert
            // Verify correct capture request was made
            _ePaymentClientMock.Verify(
                x =>
                    x.CapturePaymentAsync(
                        paymentId.ToString(),
                        It.Is<CaptureModificationRequest>(req =>
                            req.ModificationAmount.Currency == Currency.DKK
                            && req.ModificationAmount.Value == captureAmount * 100
                        )
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "CancelPayment calls API correctly")]
        public async Task CancelPayment_CallsApiCorrectly()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var paymentId = Guid.NewGuid();

            // Act
            await service.CancelPayment(paymentId);

            // Assert
            // Verify correct cancel request was made
            _ePaymentClientMock.Verify(
                x =>
                    x.CancelPaymentAsync(
                        paymentId.ToString(),
                        It.Is<CancelModificationRequest>(req => req.CancelTransactionOnly == true)
                    ),
                Times.Once
            );
        }

        [Fact(DisplayName = "ConvertToAmount correctly converts kroner to øre")]
        public async Task ConvertToAmount_CorrectlyConvertsKronerToOre()
        {
            // Arrange
            var service = new MobilePayPaymentsService(
                _ePaymentClientMock.Object,
                _mobilePaySettings,
                _loggerMock.Object
            );

            var paymentId = Guid.NewGuid();
            var amountInKroner = 50;
            var expectedAmountInOre = 5000; // 50 * 100

            // Act
            await service.CapturePayment(paymentId, amountInKroner);

            // Assert
            // Verify amount was converted correctly (kroner to øre)
            _ePaymentClientMock.Verify(
                x =>
                    x.CapturePaymentAsync(
                        paymentId.ToString(),
                        It.Is<CaptureModificationRequest>(req =>
                            req.ModificationAmount.Currency == Currency.DKK
                            && req.ModificationAmount.Value == expectedAmountInOre
                        )
                    ),
                Times.Once
            );
        }
    }
}
