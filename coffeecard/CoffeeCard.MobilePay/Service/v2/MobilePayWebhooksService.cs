using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.MobilePay.Clients;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using Microsoft.Extensions.Logging;
using ApiException = CoffeeCard.MobilePay.Generated.Api.WebhooksApi.ApiException;

namespace CoffeeCard.MobilePay.Service.v2
{
    public class MobilePayWebhooksService(WebhooksClient webhooksClient, MobilePaySettingsV3 mobilePaySettings)
        : IMobilePayWebhooksService
    {
        private readonly ILogger<MobilePayWebhooksService> _logger;

        private static readonly ISet<WebhookEvent> DefaultEvents = new HashSet<WebhookEvent>
            { WebhookEvent.Captured, WebhookEvent.Expired, WebhookEvent.Cancelled };

        public async Task<RegisterWebhookResponse> RegisterWebhook(string url)
        {
            try
            {
                _logger.LogInformation("Register new webhook for Url: {url}, Events: {@events}", url, DefaultEvents);

                var response = await webhooksClient.CreateWebhookAsync(new RegisterRequest()
                {
                    Events = DefaultEvents.Select(webhookEvent => webhookEvent.ToMpEventType()).ToList(),
                    Url = new Uri(url)
                });

                return new RegisterWebhookResponse()
                {
                    WebhookId = response.Id,
                    Url = new Uri(url),
                    Secret = response.Secret
                };
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "Error calling Post Webhook with Url: {Url} and Events: {@Events}. Http {StatusCode} {Message}", url, DefaultEvents, e.StatusCode, e.Message);
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

        public async Task<GetWebhookResponse> GetWebhook(Guid webhookId)
        {
            try
            {
                var allWebhooks = await webhooksClient.GetAllWebhooksAsync();
                var result = allWebhooks.Webhooks.FirstOrDefault(webhook => webhook.Id == webhookId);

                if (result == null)
                {
                    _logger.LogError("Webhook with Id: {Id} does not exist", webhookId);
                    throw new EntityNotFoundException($"Webhook with Id: {webhookId} does not exist");
                }

                return new GetWebhookResponse()
                {
                    Url = result.Url,
                    WebhookId = result.Id,
                };
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "Error calling Get Webhook with Id: {Id}. Http {StatusCode} {Message}", webhookId, e.StatusCode, e.Message);
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

    }
}