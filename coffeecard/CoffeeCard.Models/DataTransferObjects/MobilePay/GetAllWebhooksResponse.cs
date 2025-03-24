using System.Collections.Generic;

namespace CoffeeCard.Models.DataTransferObjects.MobilePay;

public class GetAllWebhooksResponse
{
    public ICollection<GetWebhookResponse> Webhooks { get; set; }
}