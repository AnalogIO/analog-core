using CoffeeCard.Library.Services.v2;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCardApi.Integrations.Nexi;

public class NexiNotifcation
{
    public string Id { get; set; }
    public string Event { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public int MerchantId { get; set; }
    public int MerchantNumber { get; set; }
    public object Data { get; set; } // TODO, fix
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
    public async Task<IActionResult> ReceiveNotification(NexiNotifcation notification, [FromHeader(Name = "Authorization")] string authToken )
    {
        
        return Ok("");
    }
}