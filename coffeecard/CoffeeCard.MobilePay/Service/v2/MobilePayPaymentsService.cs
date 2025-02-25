using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Service.v2
{
    public class MobilePayPaymentsService : IMobilePayPaymentsService
    {
        private readonly MobilePaySettingsV3 _mobilePaySettings;
        private readonly MobilePaySettingsV2 _mobilePaySettingsv2;
        private readonly ePaymentClient _ePaymentApi;
        private readonly ILogger<MobilePayPaymentsService> _logger;

        public MobilePayPaymentsService(
            ePaymentClient ePaymentApi,
            MobilePaySettingsV3 mobilePaySettings,
            MobilePaySettingsV2 mobilePaySettingsv2, 
            ILogger<MobilePayPaymentsService> logger
        )
        {
            _ePaymentApi = ePaymentApi;
            _mobilePaySettings = mobilePaySettings;
            _mobilePaySettingsv2 = mobilePaySettingsv2;
            _logger = logger;
        }

        public async Task<MobilePayPaymentDetails> InitiatePayment(
            MobilePayPaymentRequest paymentRequest
        )
        {
            try
            {
                var orderId = paymentRequest.OrderId.ToString();
                var response = await _ePaymentApi.CreatePaymentAsync(
                    new CreatePaymentRequest
                    {
                        Amount = ConvertToAmount(paymentRequest.Amount),
                        PaymentMethod = new PaymentMethod { Type = PaymentMethodType.WALLET },
                        Reference = orderId,
                        UserFlow = CreatePaymentRequestUserFlow.WEB_REDIRECT,
                        // Unsure if return url is needed with given user flow
                        ReturnUrl = _mobilePaySettingsv2.AnalogAppRedirectUri,
                        PaymentDescription = paymentRequest.Description
                    }
                );
                _logger.LogInformation(
                    "Created MobilePay Payment with Reference {Reference} of {OrerAmount} (DKK)",
                    response.Reference.ToString(),
                    paymentRequest.Amount
                );
                
                var test = response.RedirectUrl;
                return new MobilePayPaymentDetails
                {
                    MobilePayAppRedirectUri = response.RedirectUrl.ToString(),
                    PaymentId = orderId
                };
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                _logger.LogError(
                    e,
                    "MobilePay CreatePayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message}",
                    e.StatusCode,
                    errorResponse.Status,
                    e.Message
                );

                // FIXME Consider retry

                throw new MobilePayApiException(e.StatusCode, e.Message);
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
                var response = await _ePaymentApi.GetPaymentAsync(paymentId.ToString());

                return new MobilePayPaymentDetails
                {
                    PaymentId = response.Reference,
                    MobilePayAppRedirectUri = response.RedirectUrl.ToString(),
                };
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                _logger.LogError(
                    e,
                    "MobilePay GetPayment failed with HTTP {StatusCode}. ErrorCode: {ErrorCode} Message: {Message}",
                    e.StatusCode,
                    e.StatusCode,
                    e.Message
                );

                // FIXME Consider retry

                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
            catch (ApiException apiException)
            {
                LogMobilePayException(apiException);
                throw new MobilePayApiException(apiException.StatusCode, apiException.Message);
            }
        }

        public async Task<bool> RefundPayment(Purchase purchase, int amount)
        {
            if (purchase == null || purchase.ExternalTransactionId == null) throw new ArgumentNullException(nameof(purchase));
            try
            {
                var issueRefundRequest = new RefundModificationRequest
                {
                    ModificationAmount = new Amount { Currency = Currency.DKK, Value = amount },
                };
                try
                {
                    var response = await _ePaymentApi.RefundPaymentAsync(
                        reference: purchase.ExternalTransactionId.ToString(),
                        issueRefundRequest
                    );
                    return true; // TODO: Do we want to return the response?
                }
                catch (ApiException e)
                {
                    _logger.LogError(
                        e,
                        "MobilePay RefundPayment failed with HTTP {StatusCode}. Message: {Message}",
                        e.StatusCode,
                        e.Message
                    );
                    return false;
                }
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                _logger.LogError(
                    e,
                    "MobilePay RefundPayment failed with HTTP {StatusCode}. Message: {Message}",
                    e.StatusCode,
                    e.Message
                );
                throw new MobilePayApiException(
                    e.StatusCode,
                    e.Message
                );
            }
        }

        public async Task CapturePayment(Guid paymentId, int amountInDanishKroner)
        {
            try
            {
                await _ePaymentApi.CapturePaymentAsync(
                    reference: paymentId.ToString(),
                    new CaptureModificationRequest { ModificationAmount = ConvertToAmount(amountInDanishKroner) }
                );
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                _logger.LogError(
                    e,
                    "MobilePay CapturePayment failed with HTTP {StatusCode}. Message: {Message}",
                    e.StatusCode,
                    e.Message
                );

                // FIXME Consider retry

                throw new MobilePayApiException(
                    e.StatusCode,
                    e.Message
                );
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
                await _ePaymentApi.CancelPaymentAsync(
                    reference: paymentId.ToString(),
                    new CancelModificationRequest
                    {
                        CancelTransactionOnly = true
                    }
                );
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                _logger.LogError(
                    e,
                    "MobilePay CancelPayment failed with HTTP {StatusCode}. Message: {Message}",
                    e.StatusCode,
                    e.Message
                );

                // FIXME Consider retry

                throw new MobilePayApiException(
                    e.StatusCode,
                    e.Message
                );
            }
            catch (ApiException apiException)
            {
                LogMobilePayException(apiException);
                throw new MobilePayApiException(apiException.StatusCode, apiException.Message);
            }
        }

        /// <summary>
        /// Convert Amount (in whole DKK kroner) to MobilePay-compatible amount
        /// </summary>
        /// <param name="amountInKroner">Amount in Danish kroner</param>
        /// <returns>MobilePay-compatible Amount</returns>
        private Amount ConvertToAmount(int amountInKroner) =>
            new Amount { Currency = Currency.DKK, Value = amountInKroner * 100 };

        private void LogMobilePayException(ApiException apiException)
        {
            _logger.LogError(
                apiException,
                "MobilePay InitiatePayment failed with HTTP {StatusCode}. Message: {Message}",
                apiException.StatusCode,
                apiException.Message
            );
        }
    }
}
