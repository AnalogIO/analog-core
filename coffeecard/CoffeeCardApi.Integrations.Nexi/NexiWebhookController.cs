using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCardApi.Integrations.Nexi;

public class NexiNotifcation
{
    public string Id { get; set; }
    public string Event { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int MerchantId { get; set; }
    public int MerchantNumber { get; set; }
    public Details Data { get; set; } // TODO, fix
}

public class Details
{
    public string PaymentId { get; set; }
}

[Controller]
public class NexiWebhookController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public NexiWebhookController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost]
    [Route("/nexi/webhook")]
    public async Task<IActionResult> ReceiveNotification(
        NexiNotifcation notification,
        [FromHeader(Name = "Authorization")] string authToken
    )
    {
        //TODO ADD auth validation
        var notificationType = notification.Event switch
        {
            NexiEventNames.PaymentCheckoutCompleted => WebhookNotification.Authorized,
            NexiEventNames.PaymentCancelFailed => WebhookNotification.Aborted,
            NexiEventNames.PaymentCancelCreated => WebhookNotification.Cancelled,
            NexiEventNames.PaymentChargeCreated => WebhookNotification.Captured,
            NexiEventNames.PaymentChargeCreatedV2 => WebhookNotification.Captured,
            NexiEventNames.PaymentChargeFailed => WebhookNotification.Aborted,
            NexiEventNames.PaymentChargeFailedV2 => WebhookNotification.Aborted,
            NexiEventNames.PaymentCreated => WebhookNotification.Authorized,
            NexiEventNames.PaymentRefundCompleted => WebhookNotification.Refunded,
            NexiEventNames.PaymentRefundFailed => WebhookNotification.Aborted,
            NexiEventNames.PaymentRefundInitiated => WebhookNotification.Refunded,
            NexiEventNames.PaymentRefundInitiatedV2 => WebhookNotification.Refunded,
            NexiEventNames.PaymentReservationCreated => WebhookNotification.Authorized,
            NexiEventNames.PaymentReservationCreatedV2 => WebhookNotification.Authorized,
            NexiEventNames.PaymentReservationFailed => WebhookNotification.Aborted,
            _ => throw new BadRequestException($"Event Type {notification.Event} is not valid"),
        };

        await _purchaseService.HandleWebhookPaymentUpdate(
            notification.Data.PaymentId,
            notificationType,
            PaymentType.Nexi
        );

        return Ok();
    }
}