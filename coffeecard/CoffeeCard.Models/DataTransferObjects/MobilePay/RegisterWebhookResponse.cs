using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay;

public class RegisterWebhookResponse
{
    [Required]
    public required Uri Url { get; set; }

    [Required]
    public required Guid WebhookId { get; set; }

    [Required]
    public required string Secret { get; set; }
}