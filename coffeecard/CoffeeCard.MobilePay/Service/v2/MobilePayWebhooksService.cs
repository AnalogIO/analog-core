using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<SingleWebhookResponse> RegisterWebhook(
            string url,
            ICollection<Events> events
        )
        {
            try
            {
                Log.Information(
                    "Register new webhook for Url: {url}, Events: {events}",
                    url,
                    events
                );

                return await _webhooksApi.CreateWebhookAsync(
                    new CreateWebhookRequest { Events = events, Url = url }
                );
            }
            catch (ApiException e)
            {
                Log.Error(
                    e,
                    "Error calling Post Webhook with Url: {Url} and Events: {Events}. Http {StatusCode} {Message}",
                    url,
                    events,
                    e.StatusCode,
                    e.Message
                );
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
                        Log.Error(
                            e,
                            "Error calling Get Webhook with Id: {Id}. Http {StatusCode} {Message}",
                            webhookId,
                            e.StatusCode,
                            e.Message
                        );
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
                Log.Error(
                    e,
                    "Error calling Delete Webhook with Id: {Id}. Http {StatusCode} {Message}",
                    webhookId,
                    e.StatusCode,
                    e.Message
                );
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

        public async Task<GetMultipleWebhooksResponse> GetAllWebhooks()
        {
            try
            {
                return await _webhooksApi.GetWebhooksListAsync();
            }
            catch (ApiException e)
            {
                Log.Error(
                    "Error calling GetAllWebhooks. Http {StatusCode} {Message}",
                    e.StatusCode,
                    e.Message
                );
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

        public async Task<SingleWebhookResponse> UpdateWebhook(
            Guid webhookId,
            string url,
            ICollection<Events> events
        )
        {
            var events3 = events.Select(MapEventsToEvents3).ToHashSet();

            Log.Information(
                "Sync webhook subscription. Webhook: {webhookId}, Url: {url}, subscribed webhook events: {events}",
                webhookId,
                url,
                events3
            );

            try
            {
                return await _webhooksApi.UpdateWebhookAsync(
                    webhookId,
                    new UpdateWebhookRequest { Url = url, Events = events3 }
                );
            }
            catch (ApiException e)
            {
                Log.Error(
                    e,
                    "Error calling Update Webhook with Id: {Id}. Http {StatusCode} {Message}",
                    webhookId,
                    e.StatusCode,
                    e.Message
                );
                throw new MobilePayApiException(e.StatusCode, e.Message);
            }
        }

        private static Events3 MapEventsToEvents3(Events events)
        {
            var event3 = events switch
            {
                Events.Payment_cancelled_by_user => Events3.Payment_cancelled_by_user,
                Events.Payment_expired => Events3.Payment_expired,
                Events.Payment_reserved => Events3.Payment_reserved,
                Events.Paymentpoint_activated => Events3.Paymentpoint_activated,
                Events.Transfer_succeeded => Events3.Transfer_succeeded,
                _ => throw new ArgumentException($"Events value {events} not mapped to Events3")
            };

            return event3;
        }
    }
}
