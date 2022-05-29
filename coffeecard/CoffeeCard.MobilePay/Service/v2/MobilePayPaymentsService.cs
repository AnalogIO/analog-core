﻿using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.PaymentsApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Serilog;

namespace CoffeeCard.MobilePay.Service.v2
{
    public class MobilePayPaymentsService : IMobilePayPaymentsService
    {
        private readonly MobilePaySettingsV2 _mobilePaySettings;
        private readonly PaymentsApi _paymentsApi;

        public MobilePayPaymentsService(PaymentsApi paymentsApi, MobilePaySettingsV2 mobilePaySettings)
        {
            _paymentsApi = paymentsApi;
            _mobilePaySettings = mobilePaySettings;
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
                    RedirectUri = "analogcoffeecard://mobilepay_purchase", // FIXME
                    Reference = paymentRequest.OrderId.ToString(),
                    Description = paymentRequest.Description
                });

                return new MobilePayPaymentDetails(paymentRequest.OrderId.ToString(), response.MobilePayAppRedirectUri,
                    response.PaymentId.ToString(), null);
            }
            catch (ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay InitiatePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayApiException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException apiException)
            {
                LogMobilePayException(apiException);
                throw new MobilePayApiException(apiException.StatusCode, apiException.Message);
            }
        }

        public async Task<MobilePayPaymentDetails> GetPayment(Guid paymentId)
        {
            try
            {
                var response = await _paymentsApi.PaymentsGET2Async(paymentId, null);

                return new MobilePayPaymentDetails(response.Reference, response.RedirectUri,
                    response.PaymentId.ToString(), response.State.ToString());
            }
            catch (ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay GetPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayApiException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException apiException)
            {
                LogMobilePayException(apiException);
                throw new MobilePayApiException(apiException.StatusCode, apiException.Message);
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
            catch (ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay CapturePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayApiException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException apiException)
            {
                LogMobilePayException(apiException);
                throw new MobilePayApiException(apiException.StatusCode, apiException.Message);
            }
        }

        public async Task CancelPayment(Guid paymentId)
        {
            try
            {
                await _paymentsApi.CancelAsync(paymentId, null);
            }
            catch (ApiException<ErrorResponse> e)
            {
                var errorResponse = e.Result;
                Log.Error(
                    "MobilePay CancelPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message} CorrelationId: {CorrelationId}",
                    e.StatusCode, errorResponse.Code, errorResponse.Message, errorResponse.CorrelationId);

                // FIXME Consider retry

                throw new MobilePayApiException(e.StatusCode, errorResponse.Message, errorResponse.Code);
            }
            catch (ApiException apiException)
            {
                LogMobilePayException(apiException);
                throw new MobilePayApiException(apiException.StatusCode, apiException.Message);
            }
        }

        /// <summary>
        /// Convert Amount in kroner to amount in Danish ører
        /// </summary>
        /// <param name="amountInKroner">Amount in Danish kroner</param>
        /// <returns>Amount in Danish ører</returns>
        private static int ConvertAmountToOrer(int amountInKroner) => amountInKroner * 100;
        
        private void LogMobilePayException(ApiException apiException) 
        {
            Log.Error(
                "MobilePay InitiatePayment failed with HTTP {StatusCode}. Message: {Message}",
                apiException.StatusCode, apiException.Message);
        }
    }
}