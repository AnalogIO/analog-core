using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay;

/// <summary>
/// Response object returned after successfully registering a new MobilePay webhook.
/// </summary>
public class RegisterWebhookResponse
{
    /// <summary>
    /// The URL to which MobilePay will send webhook notifications.
    /// </summary>
    [Required]
    public required Uri Url { get; set; }

    /// <summary>
    /// The unique identifier of the newly registered webhook.
    /// </summary>
    [Required]
    public required Guid WebhookId { get; set; }

    /// <summary>
    /// The secret key used to verify the authenticity of incoming webhook requests.
    /// </summary>
    [Required]
    public required string Secret { get; set; }
}
