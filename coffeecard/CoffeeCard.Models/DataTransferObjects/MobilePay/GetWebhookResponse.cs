using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay;

/// <summary>
/// Response object containing information about a specific MobilePay webhook.
/// </summary>
public class GetWebhookResponse
{
    /// <summary>
    /// The URL to which MobilePay will send webhook notifications.
    /// </summary>
    [Required]
    public required Uri Url { get; set; }

    /// <summary>
    /// The unique identifier of the registered webhook.
    /// </summary>
    [Required]
    public required Guid WebhookId { get; set; }
}