using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Serilog;

namespace CoffeeCard.MobilePay.Service.v2
{
    public class MobilePayPaymentsService : IMobilePayPaymentsService
    {
        private readonly MobilePaySettingsV2 _mobilePaySettings;
        private readonly ePaymentApi _ePaymentApi;

        public MobilePayPaymentsService(
            ePaymentApi ePaymentApi,
            MobilePaySettingsV2 mobilePaySettings
        )
        {
            _ePaymentApi = ePaymentApi;
            _mobilePaySettings = mobilePaySettings;
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
                        UserFlow = CreatePaymentRequestUserFlow.NATIVE_REDIRECT,
                        // Unsure if return url is needed with given user flow
                        ReturnUrl = _mobilePaySettings.AnalogAppRedirectUri,
                        PaymentDescription = paymentRequest.Description
                    },
                    idempotency_Key: orderId,
                    ocp_Apim_Subscription_Key: _mobilePaySettings.OcpApimSubscriptionKey,
                    merchant_Serial_Number: _mobilePaySettings.MerchantSerialNumber,
                    vipps_System_Name: null,
                    vipps_System_Version: null,
                    vipps_System_Plugin_Name: null,
                    vipps_System_Plugin_Version: null
                );

                Log.Information(
                    "Created MobilePay Payment with Reference {Reference} of {OrerAmount} (DKK)",
                    response.Reference.ToString(),
                    paymentRequest.Amount
                );

                return new MobilePayPaymentDetails
                {
                    MobilePayAppRedirectUri = response.RedirectUrl.ToString(),
                    PaymentId = orderId
                };
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                Log.Error(
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
                var response = await _ePaymentApi.GetPaymentAsync(
                    paymentId.ToString(),
                    merchant_Serial_Number: _mobilePaySettings.MerchantSerialNumber,
                    ocp_Apim_Subscription_Key: _mobilePaySettings.OcpApimSubscriptionKey
                );

                return new MobilePayPaymentDetails
                {
                    PaymentId = response.Reference,
                    MobilePayAppRedirectUri = response.RedirectUrl.ToString(),
                };
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                Log.Error(
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
                        issueRefundRequest,
                        reference: purchase.ExternalTransactionId.ToString(),
                        merchant_Serial_Number: _mobilePaySettings.MerchantSerialNumber,
                        ocp_Apim_Subscription_Key: _mobilePaySettings.OcpApimSubscriptionKey,
                        idempotency_Key: purchase.ExternalTransactionId.ToString(),
                        vipps_System_Name: null,
                        vipps_System_Version: null,
                        vipps_System_Plugin_Name: null,
                        vipps_System_Plugin_Version: null
                    );
                    return true;
                }
                catch (ApiException e)
                {
                    Log.Error(
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
                Log.Error(
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
                    new CaptureModificationRequest { ModificationAmount = ConvertToAmount(amountInDanishKroner) },
                    reference: paymentId.ToString(),
                    merchant_Serial_Number: _mobilePaySettings.MerchantSerialNumber,
                    ocp_Apim_Subscription_Key: _mobilePaySettings.OcpApimSubscriptionKey,
                    idempotency_Key: paymentId.ToString(),
                    vipps_System_Name: null,
                    vipps_System_Version: null,
                    vipps_System_Plugin_Name: null,
                    vipps_System_Plugin_Version: null
                );
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                Log.Error(
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
                    new CancelModificationRequest {
                        CancelTransactionOnly = true
                    },
                    reference: paymentId.ToString(),
                    merchant_Serial_Number: _mobilePaySettings.MerchantSerialNumber,
                    ocp_Apim_Subscription_Key: _mobilePaySettings.OcpApimSubscriptionKey,
                    idempotency_Key: paymentId.ToString(),
                    vipps_System_Name: null,
                    vipps_System_Version: null,
                    vipps_System_Plugin_Name: null,
                    vipps_System_Plugin_Version: null
                );
            }
            catch (ApiException<Problem> e)
            {
                var errorResponse = e.Result;
                Log.Error(
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
            Log.Error(
                apiException,
                "MobilePay InitiatePayment failed with HTTP {StatusCode}. Message: {Message}",
                apiException.StatusCode,
                apiException.Message
            );
        }
    }
}
