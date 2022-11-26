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
        
        public async Task<SingleWebhookResponse> RegisterWebhook(string url, ICollection<Events> events)
        {
            try
            {
                return await _webhooksApi.CreateWebhookAsync( new CreateWebhookRequest
                {
                    Events = events,
                    Url = url
                });
            }
            catch (ApiException e)
            {
                Log.Error("Error calling Post Webhook with Url: {Url} and Events: {Events}. Http {StatusCode} {Message}", url, events, e.StatusCode, e.Message);
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

        public async Task<SingleWebhookResponse> GetWebhook(Guid webhookId)
        {
            try
            {
                return await _webhooksApi.GetWebhookAsync(webhookId);
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
                await _webhooksApi.DeleteWebhookAsync(webhookId);
            }
            catch (ApiException e)
            {
                Log.Error("Error calling Get Webhook with Id: {Id}. Http {StatusCode} {Message}", webhookId, e.StatusCode, e.Message);
                throw new MobilePayApiException(e.StatusCode, e.Message);

            }
        }
    }
}