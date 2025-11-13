using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;
using Microsoft.Extensions.Logging;
using ApiException = CoffeeCard.MobilePay.Generated.Api.WebhooksApi.ApiException;

namespace CoffeeCard.MobilePay.Service.v2;

public class MobilePayWebhooksService(
    IWebhooksClient webhooksClient,
    ILogger<MobilePayWebhooksService> logger
) : IMobilePayWebhooksService
{
    private static readonly ISet<WebhookEvent> DefaultEvents = new HashSet<WebhookEvent>
    {
        WebhookEvent.Authorized,
        WebhookEvent.Cancelled,
        WebhookEvent.Expired,
        WebhookEvent.Aborted,
    };

    public async Task<RegisterWebhookResponse> RegisterWebhook(string url)
    {
        logger.LogInformation(
            "Register new webhook for Url: {url}, Events: {@events}",
            url,
            DefaultEvents
        );

        var response = await webhooksClient.CreateWebhookAsync(
            new RegisterRequest()
            {
                Events = DefaultEvents
                    .Select(webhookEvent => webhookEvent.ToMpEventType())
                    .ToList(),
                Url = new Uri(url),
            }
        );

        return new RegisterWebhookResponse()
        {
            WebhookId = response.Id,
            Url = new Uri(url),
            Secret = response.Secret,
        };
    }

    public async Task<GetWebhookResponse> GetWebhook(Guid webhookId)
    {
        var allWebhooks = await webhooksClient.GetAllWebhooksAsync();
        var result = allWebhooks.Webhooks.FirstOrDefault(webhook => webhook.Id == webhookId);

        if (result == null)
        {
            logger.LogError("Webhook with Id: {Id} does not exist", webhookId);
            throw new EntityNotFoundException($"Webhook with Id: {webhookId} does not exist");
        }

        return new GetWebhookResponse() { Url = result.Url, WebhookId = result.Id };
    }

    public async Task<GetAllWebhooksResponse> GetAllWebhooks()
    {
        var allWebhooks = await webhooksClient.GetAllWebhooksAsync();
        return new GetAllWebhooksResponse()
        {
            Webhooks = allWebhooks
                .Webhooks.Select(webhook => new GetWebhookResponse()
                {
                    Url = webhook.Url,
                    WebhookId = webhook.Id,
                })
                .ToList(),
        };
    }
}
