using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay;

/// <summary>
/// Response object containing information about a specific MobilePay webhook.
/// </summary>
public class GetWebhookResponse
{
    /// <summary>
    /// The URL to which MobilePay will send webhook notifications.
    /// </summary>
    /// <example>
    /// https://core.dev.analogio.dk/api/v2/mobilepay
    /// </example>
    [Required]
    public required Uri Url { get; set; }

    /// <summary>
    /// The unique identifier of the registered webhook.
    /// </summary>
    /// <example>
    /// 3b69088a-0d86-4c24-9187-351e478513fd
    /// </example>
    [Required]
    public required Guid WebhookId { get; set; }
}
