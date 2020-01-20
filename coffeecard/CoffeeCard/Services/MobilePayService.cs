using System.Net;
using System.Threading.Tasks;
using CoffeeCard.Helpers.MobilePay;
using CoffeeCard.Helpers.MobilePay.RequestMessage;
using CoffeeCard.Helpers.MobilePay.ResponseMessage;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CoffeeCard.Services
{
    public class MobilePayService : IMobilePayService
    {
        private readonly IMobilePayApiHttpClient _mobilePayAPIClient;
        private readonly string _merchantId;

        public MobilePayService(IMobilePayApiHttpClient mobilePayAPIClient, IConfiguration configuration)
        {
            _mobilePayAPIClient = mobilePayAPIClient;
            _merchantId = configuration["MPMerchantID"];
        }

        public async Task<GetPaymentStatusResponse> GetPaymentStatus(string orderId)
        {
            Log.Information($"Checking order against MobilePay with orderId = {orderId}");

            try
            {
                var response = await _mobilePayAPIClient.SendRequest<GetPaymentStatusResponse>(new GetPaymentStatusRequest(_merchantId, orderId));

                Log.Information($"MobilePay transactionId = {response.TransactionId}, orderId = {orderId} has PaymentStatus = {response.LatestPaymentStatus}");

                return response;
            }
            catch (MobilePayException exception)
            {
                // If exception is InternalServerError or RequestTimeOut, we try one more time since the MobilePay API
                // in rare case (according to documentation) can fail with one of these mistakes and should retry if so
                if (!exception.GetHttpStatusCode().Equals(HttpStatusCode.InternalServerError) &&
                    !exception.GetHttpStatusCode().Equals(HttpStatusCode.RequestTimeout)) throw;
                
                try
                {
                    Log.Warning($"Retrying to retrieve Payment Status. Last call failed with {exception}");
                    
                    var response = await _mobilePayAPIClient.SendRequest<GetPaymentStatusResponse>(new GetPaymentStatusRequest(_merchantId, orderId));
                    return response;
                }
                catch (MobilePayException retryException)
                {
                    Log.Error($"Retry failed. Exception thrown = {retryException}");
                    throw;
                }
            }
        }

        public async Task<CaptureAmountResponse> CapturePayment(string orderId)
        {
            try
            {
                var response = await _mobilePayAPIClient.SendRequest<CaptureAmountResponse>(new CaptureAmountRequest(_merchantId, orderId));
                Log.Information($"MobilePay transactionId = {response.TransactionId}, orderId = {orderId} has been captured from the MobilePay API");
                return response;
            }
            catch (MobilePayException exception)
            {
                if (!exception.GetHttpStatusCode().Equals(HttpStatusCode.InternalServerError) &&
                    !exception.GetHttpStatusCode().Equals(HttpStatusCode.RequestTimeout)) throw;
                
                var paymentStatus = await GetPaymentStatus(orderId);
                if (paymentStatus.LatestPaymentStatus.Equals(PaymentStatus.Captured))
                {
                    return new CaptureAmountResponse
                    {
                        TransactionId = paymentStatus.TransactionId
                    };
                }

                // If Transaction is not in State captured, throw the causing exception
                throw;
            }
        }

        public async Task<CancelReservationResponse> CancelPaymentReservation(string orderId)
        {
            try
            {
                var response = await _mobilePayAPIClient.SendRequest<CancelReservationResponse>(new CancelReservationRequest(_merchantId, orderId));
                Log.Information($"MobilePay transactionId = {response.TransactionId}, orderId = {orderId} has been cancelled from the MobilePay API");
                return response;
            }
            catch (MobilePayException e)
            {
                if (!e.GetHttpStatusCode().Equals(HttpStatusCode.InternalServerError) &&
                    !e.GetHttpStatusCode().Equals(HttpStatusCode.RequestTimeout)) throw;
                
                var paymentStatus = await GetPaymentStatus(orderId);
                if (paymentStatus.LatestPaymentStatus.Equals(PaymentStatus.Cancelled))
                {
                    return new CancelReservationResponse
                    {
                        TransactionId = paymentStatus.TransactionId
                    };
                }

                // If Transaction is not in state Cancelled, throw the causing exception
                throw;
            }
        }
        
        public void Dispose()
        {
            _mobilePayAPIClient.Dispose();
        }
    }
}