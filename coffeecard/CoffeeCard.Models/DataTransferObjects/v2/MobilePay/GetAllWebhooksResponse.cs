using System.Collections.Generic;
using CoffeeCard.Models.DataTransferObjects.MobilePay;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay;

/// <summary>
/// Response object for retrieving all registered MobilePay webhooks.
/// </summary>
public class GetAllWebhooksResponse
{
    /// <summary>
    /// Collection of webhook response objects containing information about each registered webhook.
    /// </summary>
    public required ICollection<GetWebhookResponse> Webhooks { get; init; }
}
