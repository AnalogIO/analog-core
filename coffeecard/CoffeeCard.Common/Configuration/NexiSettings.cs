using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Common.Configuration;

public class NexiSettings
{
    [Required]
    public required Uri ApiUrl { get; set; }
    [Required]
    public required string ApiKey { get; set; }
    [Required]
    public required string WebhookUrl { get; set; }
}
