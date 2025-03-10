using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay;

public class GetWebhookResponse
{
    [Required]
    public required Uri Url { get; set; }

    [Required]
    public required Guid WebhookId { get; set; }
}