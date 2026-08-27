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

internal class NexiStrategy : IPaymentStrategy
{
    private readonly NexiClient _checkoutPaymentApi;
    private readonly ILogger<NexiStrategy> _logger;

    public NexiStrategy(NexiClient checkoutPaymentApi, ILogger<NexiStrategy> logger)
    {
        _checkoutPaymentApi = checkoutPaymentApi;
        _logger = logger;
    }

    public async Task<PaymentInitiationResult> InitiatePaymentAsync(
        ProductResponse product,
        Guid orderId
    )
    {
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
                        UnitPrice = product.Price / product.NumberOfTickets,
                        GrossTotalAmount = product.Price,
                        TaxRate = 2500,
                        TaxAmount = (int)(product.Price * 0.8),
                        NetTotalAmount = product.Price - (int)(product.Price * 0.8),
                    },
                ],
                Amount = product.Price,
                Currency = "DKK",
            },
            Checkout = new CheckoutDetails
            {
                TermsUrl = null,
                IntegrationType = "HostedPaymentPage",
            },
        };

        var response = await _checkoutPaymentApi.Create_paymentAsync(
            commercePlatformTag: null,
            request
        );

        return new PaymentInitiationResult(
            PurchaseStatus.PendingPayment,
            response.PaymentId,
            new NexiPaymentDetails
            {
                PaymentId =
                    response.PaymentId
                    // Replace with correct exception
                    ?? throw new MobilePayApiException(500, "Nexi transaction failed"),
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

        return new NexiPaymentDetails()
        {
            OrderId = purchase.OrderId,
            PaymentId = response.Payment.PaymentId.ToString(),
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
