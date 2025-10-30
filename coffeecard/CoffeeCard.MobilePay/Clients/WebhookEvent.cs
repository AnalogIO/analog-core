using System;

namespace CoffeeCard.MobilePay.Clients;

internal enum WebhookEvent
{
    Created = 0,
    Aborted = 1,
    Expired = 2,
    Cancelled = 3,
    Captured = 4,
    Refunded = 5,
    Authorized = 6,
    Terminated = 7,
}

internal static class WebhookEventsExtension
{
    internal static string ToMpEventType(this WebhookEvent webhookEvent)
    {
        return webhookEvent switch
        {
            WebhookEvent.Created => "epayments.payment.created.v1",
            WebhookEvent.Aborted => "epayments.payment.aborted.v1",
            WebhookEvent.Expired => "epayments.payment.expired.v1",
            WebhookEvent.Cancelled => "epayments.payment.cancelled.v1",
            WebhookEvent.Captured => "epayments.payment.captured.v1",
            WebhookEvent.Refunded => "epayments.payment.refunded.v1",
            WebhookEvent.Authorized => "epayments.payment.authorized.v1",
            WebhookEvent.Terminated => "epayments.payment.terminated.v1",
            _ => throw new ArgumentOutOfRangeException(nameof(webhookEvent), webhookEvent, null),
        };
    }
}
