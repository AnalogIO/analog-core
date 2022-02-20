using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.PaymentsApi;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Serilog;
using ApiException = CoffeeCard.MobilePay.Generated.Api.PaymentsApi.ApiException;
using ErrorResponse = CoffeeCard.MobilePay.Generated.Api.PaymentsApi.ErrorResponse;

namespace CoffeeCard.MobilePay.Service.v2
{
    public class MobilePayService : IMobilePayService
    {
        private readonly MobilePaySettingsV2 _mobilePaySettings;
        private readonly PaymentsApi _paymentsApi;
        private readonly WebhooksApi _webhooksApi;

        public MobilePayService(PaymentsApi paymentsApi, MobilePaySettingsV2 mobilePaySettings, WebhooksApi webhooksApi)
        {
            _paymentsApi = paymentsApi;
            _mobilePaySettings = mobilePaySettings;
            _webhooksApi = webhooksApi;
        }

        public async Task<MobilePayPaymentDetails> InitiatePayment(MobilePayPaymentRequest paymentRequest)
        {
            try
            {
                var response = await _paymentsApi.PaymentsPOSTAsync(null, new InitiatePaymentRequest
                {
                    Amount = ConvertAmountToOrer(paymentRequest.Amount),
                    IdempotencyKey = paymentRequest.OrderId,
                    PaymentPointId = _mobilePaySettings.PaymentPointId,
                    RedirectUri = "", // FIXME
                    Reference = paymentRequest.OrderId.ToString(),
                    Description = paymentRequest.Description
                });

                return new MobilePayPaymentDetails(paymentRequest.OrderId.ToString(), response.MobilePayAppRedirectUri,
                    response.PaymentId.ToString(), null);
            }
            catch (Generated.Api.PaymentsApi.ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay InitiatePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException e)
            {
                Log.Error(
                    "MobilePay InitiatePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, e.Message);

                throw new MobilePayException(e.StatusCode, e.Message);
            }
        }

        public async Task<MobilePayPaymentDetails> GetPayment(Guid paymentId)
        {
            try
            {
                var response = await _paymentsApi.PaymentsGET2Async(paymentId, null);

                return new MobilePayPaymentDetails(response.Reference, response.MobilePayAppRedirectUri,
                    response.PaymentId.ToString(), response.State.ToString());
            }
            catch (Generated.Api.PaymentsApi.ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay GetPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException e)
            {
                Log.Error(
                    "MobilePay GetPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, e.Message);

                throw new MobilePayException(e.StatusCode, e.Message);
            }
        }

        public async Task CapturePayment(Guid paymentId, int amountInDanishKroner)
        {
            try
            {
                await _paymentsApi.CaptureAsync(paymentId, null, new CapturePaymentRequest
                {
                    Amount = ConvertAmountToOrer(amountInDanishKroner)
                });
            }
            catch (Generated.Api.PaymentsApi.ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay CapturePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException e)
            {
                Log.Error(
                    "MobilePay CapturePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, e.Message);

                throw new MobilePayException(e.StatusCode, e.Message);
            }
        }

        public async Task CancelPayment(Guid paymentId)
        {
            try
            {
                await _paymentsApi.CancelAsync(paymentId, null);
            }
            catch (Generated.Api.PaymentsApi.ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay CancelPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException e)
            {
                Log.Error(
                    "MobilePay CancelPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, e.Message);

                throw new MobilePayException(e.StatusCode, e.Message);
            }
        }

        public async Task RegisterWebhook()
        {
            Log.Information("Register Webhook at MobilePay for Events: {Events} and Url: {Uri}", null, null);
            await _webhooksApi.WebhooksPOSTAsync(null, new CreateWebhookRequest
            {
                Events = new List<string> { "payment.reserved", "payment.expired" },
                Url = ""
            });
        }

        public async Task DeregisterWebhook()
        {
            Log.Information("Deregister MobilePay Webhook with Id: {Id}", null);
            // FIXME
            await _webhooksApi.WebhooksDELETEAsync(Guid.Empty, null);
        }

        /// <summary>
        /// Convert Amount in kroner to amount in Danish ører
        /// </summary>
        /// <param name="amountInKroner">Amount in Danish kroner</param>
        /// <returns>Amount in Danish ører</returns>
        private static int ConvertAmountToOrer(int amountInKroner) => amountInKroner * 100;
    }
}