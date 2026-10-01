using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2.PaymentStrategies;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using CoffeeCardApi.Integrations.Nexi.Generated.Client;
using Microsoft.Extensions.Logging;
using PaymentDetails = CoffeeCard.Models.DataTransferObjects.v2.Purchase.PaymentDetails;

namespace CoffeeCardApi.Integrations.Nexi;

internal static class NexiEventNames
{
    public const string PaymentCheckoutCompleted = "payment.checkout.completed";
    public const string PaymentCancelFailed = "payment.cancel.failed";
    public const string PaymentCancelCreated = "payment.cancel.created";
    public const string PaymentChargeCreated = "payment.charge.created";
    public const string PaymentChargeCreatedV2 = "payment.charge.created.v2";
    public const string PaymentChargeFailed = "payment.charge.failed";
    public const string PaymentChargeFailedV2 = "payment.charge.failed.v2";
    public const string PaymentCreated = "payment.created";
    public const string PaymentRefundCompleted = "payment.refund.completed";
    public const string PaymentRefundFailed = "payment.refund.failed";
    public const string PaymentRefundInitiated = "payment.refund.initiated";
    public const string PaymentRefundInitiatedV2 = "payment.refund.initiated.v2";
    public const string PaymentReservationCreated = "payment.reservation.created";
    public const string PaymentReservationCreatedV2 = "payment.reservation.created.v2";
    public const string PaymentReservationFailed = "payment.reservation.failed";
}

internal class NexiStrategy : IPaymentStrategy
{
    private readonly NexiClient _checkoutPaymentApi;
    private readonly ILogger<NexiStrategy> _logger;
    private readonly NexiSettings _settings;

    public NexiStrategy(NexiClient checkoutPaymentApi, ILogger<NexiStrategy> logger)
    {
        _checkoutPaymentApi = checkoutPaymentApi;
        _logger = logger;
    }

    private const int TaxRate = 2500;
    public async Task<PaymentInitiationResult> InitiatePaymentAsync(
        ProductResponse product,
        Guid orderId
    )
    {
        var priceInOere = product.Price * 100;

        // Calculations done according to formulas from Nexi OpenApi spec
        var unitPrice = priceInOere / product.NumberOfTickets * 10_000 / (10_000 * TaxRate);
        var taxAmount = unitPrice * product.NumberOfTickets * TaxRate / 10_000;
        var netTotal = unitPrice * product.NumberOfTickets;
        var grossTotal = netTotal + taxAmount;
        
        var request = new CreatePaymentBody()
        {
            Order = new Order
            {
                Items =
                [
                    new OrderItem
                    {
                        // TODO, consider moving to constructor/factory to better model business rules
                        Reference = product.Id.ToString(),
                        Name = product.Name,
                        Quantity = product.NumberOfTickets,
                        Unit = "Pc(s)",
                        UnitPrice = unitPrice,
                        TaxRate = TaxRate,
                        TaxAmount = taxAmount,
                        NetTotalAmount = netTotal,
                        GrossTotalAmount = grossTotal,
                    },
                ],
                Amount = priceInOere,
                Currency = "DKK",
            },
            Checkout = new CheckoutDetails
            {
                TermsUrl = null,
                IntegrationType = "HostedPaymentPage",
                
            },Notifications = new Notification()
            {
                WebHooks = [
                    new WebHook()
                    {
                        Authorization = _settings.WebhookKey,
                        EventName = NexiEventNames.PaymentCreated,
                        Url = _settings.WebhookUrl
                    }
                ]
            }
        };

        var response = await _checkoutPaymentApi.Create_paymentAsync(
            commercePlatformTag: null,
            request
        );

        if (response.PaymentId is null || response.HostedPaymentPageUrl is null)
        {
            // TODO, replace with correct exception
            throw new MobilePayApiException(500, "Nexi transaction failed");
        }

        return new PaymentInitiationResult(
            PurchaseStatus.PendingPayment,
            response.PaymentId,
            new NexiPaymentDetails
            {
                PaymentUrl = response.HostedPaymentPageUrl,
                OrderId = response.PaymentId,
            }
        );
    }

    public async Task<PaymentDetails> GetPaymentAsync(Purchase purchase)
    {
        var response = await _checkoutPaymentApi.Retrieve_paymentAsync(
            purchase.OrderId,
            commercePlatformTag: null
        );

        if (response.Payment is null)
        {
            throw new BadRequestException($"No payment found for purchase {purchase.Id}");
        }

        return new NexiPaymentDetails
        {
            OrderId = response.Payment.PaymentId.ToString(),
            PaymentUrl = response.Payment.Checkout.Url!,
        };
    }

    public Task CapturePaymentAsync(Purchase purchase)
    {
        return purchase.ExternalTransactionId is null
            ? throw new BadRequestException("No transaction id specified for purchase")
            : _checkoutPaymentApi.Retrieve_paymentAsync(
                purchase.ExternalTransactionId,
                commercePlatformTag: null
            );
    }

    public Task CancelPaymentAsync(Purchase purchase)
    {
        if (purchase.ExternalTransactionId is null)
        {
            _logger.LogWarning(
                "Attempted to cancel purchase without external transaction {PurchaseId}",
                purchase.Id
            );
            throw new BadRequestException(
                $"No transaction id specified for purchase {purchase.Id}"
            );
        }

        var request = new CancelPaymentBody { Amount = purchase.Price };

        return _checkoutPaymentApi.Cancel_paymentAsync(
            purchase.ExternalTransactionId,
            null,
            request
        );
    }

    public async Task<bool> RefundPaymentAsync(Purchase purchase)
    {
        if (purchase.ExternalTransactionId is null)
        {
            throw new BadRequestException("No transaction id specified for purchase");
        }

        var result = await _checkoutPaymentApi.Refund_chargeAsync(
            purchase.ExternalTransactionId,
            idempotency_Key: null,
            body: new RefundPaymentBody { Amount = purchase.Price }
        );

        var isSuccess = result.RefundId is not null;
        if (isSuccess)
        {
            _logger.LogInformation(
                "Refunded payment for {PurchaseExternalTransactionId} with refundId {RefundId}",
                purchase.ExternalTransactionId,
                result.RefundId
            );
        }
        else
        {
            _logger.LogError(
                "Failed to refund purchase {PurchaseExternalTransactionId}",
                purchase.ExternalTransactionId
            );
        }

        return isSuccess;
    }
}
