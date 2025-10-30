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

namespace CoffeeCard.MobilePay.Service.v2;

public class MobilePayPaymentsService(
    IEPaymentClient ePaymentClient,
    MobilePaySettings mobilePaySettings,
    ILogger<MobilePayPaymentsService> logger
) : IMobilePayPaymentsService
{
    public async Task<MobilePayPaymentDetails> InitiatePayment(
        MobilePayPaymentRequest paymentRequest
    )
    {
        var orderId = paymentRequest.OrderId.ToString();
        var response = await ePaymentClient.CreatePaymentAsync(
            new CreatePaymentRequest
            {
                Amount = ConvertToAmount(paymentRequest.Amount),
                PaymentMethod = new PaymentMethod { Type = PaymentMethodType.WALLET },
                Reference = orderId,
                UserFlow = CreatePaymentRequestUserFlow.WEB_REDIRECT,
                ReturnUrl = mobilePaySettings.AnalogAppRedirectUri,
                PaymentDescription = paymentRequest.Description,
            }
        );
        logger.LogInformation(
            "Created MobilePay Payment with Reference {Reference} of {OrerAmount} (DKK)",
            response.Reference,
            paymentRequest.Amount
        );

        return new MobilePayPaymentDetails
        {
            MobilePayAppRedirectUri = response.RedirectUrl.ToString(),
            PaymentId = orderId,
        };
    }

    public async Task<MobilePayPaymentDetails> GetPayment(Guid paymentId)
    {
        var response = await ePaymentClient.GetPaymentAsync(paymentId.ToString());

        return new MobilePayPaymentDetails
        {
            PaymentId = response.Reference,
            MobilePayAppRedirectUri = response.RedirectUrl.ToString(),
        };
    }

    public async Task<bool> RefundPayment(Purchase purchase, int amount)
    {
        if (purchase == null || purchase.ExternalTransactionId == null)
            throw new ArgumentNullException(nameof(purchase));

        var issueRefundRequest = new RefundModificationRequest
        {
            ModificationAmount = new Amount { Currency = Currency.DKK, Value = amount },
        };
        await ePaymentClient.RefundPaymentAsync(purchase.ExternalTransactionId, issueRefundRequest);

        return true;
    }

    public async Task CapturePayment(Guid paymentId, int amountInDanishKroner)
    {
        await ePaymentClient.CapturePaymentAsync(
            paymentId.ToString(),
            new CaptureModificationRequest
            {
                ModificationAmount = ConvertToAmount(amountInDanishKroner),
            }
        );
    }

    public async Task CancelPayment(Guid paymentId)
    {
        await ePaymentClient.CancelPaymentAsync(
            paymentId.ToString(),
            new CancelModificationRequest { CancelTransactionOnly = true }
        );
    }

    /// <summary>
    ///     Convert Amount (in whole DKK kroner) to MobilePay-compatible amount
    /// </summary>
    /// <param name="amountInKroner">Amount in Danish kroner</param>
    /// <returns>MobilePay-compatible Amount</returns>
    private static Amount ConvertToAmount(int amountInKroner)
    {
        return new Amount { Currency = Currency.DKK, Value = amountInKroner * 100 };
    }
}
