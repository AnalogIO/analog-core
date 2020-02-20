using System.Net;
using System.Threading.Tasks;
using CoffeeCard.Helpers.MobilePay;
using CoffeeCard.Helpers.MobilePay.ErrorMessage;
using CoffeeCard.Helpers.MobilePay.RequestMessage;
using CoffeeCard.Helpers.MobilePay.ResponseMessage;
using CoffeeCard.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CoffeeCardTests.Services
{
    public class MobilePayServiceTest
    {
        [Fact(DisplayName = "CancelPaymentReservation given orderId calls MobilePayApiHttpClient")]
        public async Task CancelPaymentReservationGivenOrderIdCallsMobilePayApiHttpClient()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var requestMessage = new CancelReservationRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<CancelReservationResponse>(
                    requestMessage))
                .ReturnsAsync(
                    new CancelReservationResponse
                    {
                        TransactionId = "transId"
                    });

            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Act
            await mobilePayService.CancelPaymentReservation("1234");

            // Assert
            mobilePayApiClient.Verify(
                m =>
                    m.SendRequest<CancelReservationResponse>(requestMessage),
                Times.Once);
        }

        [Fact(DisplayName =
            "CancelPaymentReservation rethrows exception when MobilePayApiHttpClient throws a MobilePayException with Unauthorized status code")]
        public async Task
            CancelPaymentReservationRethrowsExceptionWhenMobilePayApiHttpClientThrowsAMobilePayExceptionWithUnauthorizedStatusCode()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var requestMessage = new CancelReservationRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<CancelReservationResponse>(
                    requestMessage))
                .ThrowsAsync(
                    new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.Unauthorized));

            // Act
            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Assert
            await Assert.ThrowsAsync<MobilePayException>(() => mobilePayService.CancelPaymentReservation("1234"));
        }

        [Fact(DisplayName =
            "CancelPaymentReservation retries when MobilePayApiHttpClient throws a MobilePayException with RequestTimeOut status code")]
        public async Task
            CancelPaymentReservationRetriesWhenMobilePayApiHttpClientThrowsAMobilePayExceptionWithRequestTimeOutStatusCode()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var requestMessage = new CancelReservationRequest("merchantID", "1234");
            mobilePayApiClient
                .SetupSequence(m => m.SendRequest<CancelReservationResponse>(
                    requestMessage))
                // Throw exception first time
                .ThrowsAsync(
                    new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.RequestTimeout));

            // Second time returns Response
            var mobilePayApiRequestMessage = new GetPaymentStatusRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<GetPaymentStatusResponse>(
                    mobilePayApiRequestMessage))
                .ReturnsAsync(
                    new GetPaymentStatusResponse
                    {
                        LatestPaymentStatus = PaymentStatus.Cancelled,
                        OriginalAmount = 80.0,
                        TransactionId = "transId"
                    });


            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Act
            await mobilePayService.CancelPaymentReservation("1234");

            // Assert
            mobilePayApiClient.Verify(
                m =>
                    m.SendRequest<CancelReservationResponse>(requestMessage),
                Times.Once);
            mobilePayApiClient.Verify(
                m => m.SendRequest<GetPaymentStatusResponse>(mobilePayApiRequestMessage), Times.Once);
        }

        [Fact(DisplayName = "CapturePayment given orderId calls MobilePayApiHttpClient")]
        public async Task CapturePaymentGivenOrderIdCallsMobilePayApiHttpClient()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var requestMessage = new CaptureAmountRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<CaptureAmountResponse>(
                    requestMessage))
                .ReturnsAsync(
                    new CaptureAmountResponse
                    {
                        TransactionId = "transId"
                    });

            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Act
            await mobilePayService.CapturePayment("1234");

            // Assert
            mobilePayApiClient.Verify(
                m =>
                    m.SendRequest<CaptureAmountResponse>(requestMessage),
                Times.Once);
        }

        [Fact(DisplayName =
            "CapturePayment rethrows exception when MobilePayApiHttpClient throws a MobilePayException with Unauthorized status code")]
        public async Task
            CapturePaymentRethrowsExceptionWhenMobilePayApiHttpClientThrowsAMobilePayExceptionWithUnauthorizedStatusCode()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var requestMessage = new CaptureAmountRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<CaptureAmountResponse>(
                    requestMessage))
                .ThrowsAsync(
                    new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.Unauthorized));

            // Act
            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Assert
            await Assert.ThrowsAsync<MobilePayException>(() => mobilePayService.CapturePayment("1234"));
        }

        [Fact(DisplayName =
            "CapturePayment retries when MobilePayApiHttpClient throws a MobilePayException with RequestTimeOut status code")]
        public async Task
            CapturePaymentRetriesWhenMobilePayApiHttpClientThrowsAMobilePayExceptionWithRequestTimeOutStatusCode()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var requestMessage = new CaptureAmountRequest("merchantID", "1234");
            mobilePayApiClient
                .SetupSequence(m => m.SendRequest<CaptureAmountResponse>(
                    requestMessage))
                // Throw exception first time
                .ThrowsAsync(
                    new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.RequestTimeout));

            // Second time returns Response
            var mobilePayApiRequestMessage = new GetPaymentStatusRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<GetPaymentStatusResponse>(
                    mobilePayApiRequestMessage))
                .ReturnsAsync(
                    new GetPaymentStatusResponse
                    {
                        LatestPaymentStatus = PaymentStatus.Captured,
                        OriginalAmount = 80.0,
                        TransactionId = "transId"
                    });


            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Act
            await mobilePayService.CapturePayment("1234");

            // Assert
            mobilePayApiClient.Verify(
                m =>
                    m.SendRequest<CaptureAmountResponse>(requestMessage),
                Times.Once);
            mobilePayApiClient.Verify(
                m => m.SendRequest<GetPaymentStatusResponse>(mobilePayApiRequestMessage), Times.Once);
        }

        [Fact(DisplayName = "GetPaymentStatus given orderId calls MobilePayApiHttpClient")]
        public async Task GetPaymentStatusGivenOrderIdCallsMobilePayApiHttpClient()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var mobilePayApiRequestMessage = new GetPaymentStatusRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<GetPaymentStatusResponse>(
                    mobilePayApiRequestMessage))
                .ReturnsAsync(
                    new GetPaymentStatusResponse
                    {
                        LatestPaymentStatus = PaymentStatus.Reserved,
                        OriginalAmount = 80.0,
                        TransactionId = "transId"
                    });

            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Act
            await mobilePayService.GetPaymentStatus("1234");

            // Assert
            mobilePayApiClient.Verify(
                m =>
                    m.SendRequest<GetPaymentStatusResponse>(mobilePayApiRequestMessage),
                Times.Once);
        }

        [Fact(DisplayName =
            "GetPaymentStatus rethrows exception when MobilePayApiHttpClient throws a MobilePayException with Unauthorized status code")]
        public async Task
            GetPaymentStatusRethrowsExceptionWhenMobilePayApiHttpClientThrowsAMobilePayExceptionWithUnauthorizedStatusCode()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var mobilePayApiRequestMessage = new GetPaymentStatusRequest("merchantID", "1234");
            mobilePayApiClient
                .Setup(m => m.SendRequest<GetPaymentStatusResponse>(
                    mobilePayApiRequestMessage))
                .ThrowsAsync(
                    new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.Unauthorized));

            // Act
            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Assert
            await Assert.ThrowsAsync<MobilePayException>(() => mobilePayService.GetPaymentStatus("1234"));
        }

        [Fact(DisplayName =
            "GetPaymentStatus retries when MobilePayApiHttpClient throws a MobilePayException with RequestTimeOut status code")]
        public async Task
            GetPaymentStatusRetriesWhenMobilePayApiHttpClientThrowsAMobilePayExceptionWithRequestTimeOutStatusCode()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();

            configuration.Setup(c => c["MPMerchantID"]).Returns("merchantID");

            var mobilePayApiClient = new Mock<IMobilePayApiHttpClient>();
            var mobilePayApiRequestMessage = new GetPaymentStatusRequest("merchantID", "1234");
            mobilePayApiClient
                .SetupSequence(m => m.SendRequest<GetPaymentStatusResponse>(
                    mobilePayApiRequestMessage))
                // Throw exception first time
                .ThrowsAsync(
                    new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.RequestTimeout))
                // Second time returns Response
                .ReturnsAsync(
                    new GetPaymentStatusResponse
                    {
                        LatestPaymentStatus = PaymentStatus.Reserved,
                        OriginalAmount = 80.0,
                        TransactionId = "transId"
                    });

            var mobilePayService = new MobilePayService(mobilePayApiClient.Object, configuration.Object);

            // Act
            await mobilePayService.GetPaymentStatus("1234");

            // Assert
            mobilePayApiClient.Verify(
                m =>
                    m.SendRequest<GetPaymentStatusResponse>(mobilePayApiRequestMessage),
                Times.Exactly(2));
        }
    }
}