using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using Serilog;
using ApiException = CoffeeCard.MobilePay.Generated.Api.PaymentsApi.ApiException;

namespace CoffeeCard.MobilePay.Service.v2
{
    public class MobilePayWebhooksService : IMobilePayWebhooksService
    {
        private readonly WebhooksApi _webhooksApi;

        public MobilePayWebhooksService(WebhooksApi webhooksApi)
        {
            _webhooksApi = webhooksApi;
        }
        
        public async Task<SingleWebhookResponse> RegisterWebhook(string url, ICollection<string> events)
        {
            try
            {
                return await _webhooksApi.WebhooksPOSTAsync(null, new CreateWebhookRequest
                {
                    Events = events,
                    Url = url
                });
            }
            catch (ApiException e)
            {
                Log.Error("Error calling Post Webhook with Url: {url} and Events: {events}. Http {StatusCode} {Message}", url, events, e.StatusCode, e.Message);
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

        public async Task<SingleWebhookResponse> GetWebhook(Guid webhookId)
        {
            try
            {
                return await _webhooksApi.WebhooksGETAsync(webhookId, null);
            }
            catch (ApiException e)
            {
                switch (e.StatusCode)
                {
                    case 404:
                        Log.Error("Webhook with Id: {Id} does not exist", webhookId);
                        throw new EntityNotFoundException(e.Message);
                    default:
                        Log.Error("Error calling Get Webhook with Id: {Id}. Http {StatusCode} {Message}", webhookId, e.StatusCode, e.Message);
                        throw new MobilePayApiException(e.StatusCode, e.Message);
                }
            }
        }

        public async Task DeregisterWebhook(Guid webhookId)
        {
            try
            {
                await _webhooksApi.WebhooksDELETEAsync(webhookId, null);
            }
            catch (ApiException e)
            {
                Log.Error("Error calling Get Webhook with Id: {Id}. Http {StatusCode} {Message}", webhookId, e.StatusCode, e.Message);
                throw new MobilePayApiException(e.StatusCode, e.Message);

            }
        }
    }
}