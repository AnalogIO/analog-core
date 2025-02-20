using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;

namespace CoffeeCard.MobilePay.Service.v2
{
    public interface IMobilePayWebhooksService
    {
        /// <summary>
        /// Register Webhook at MobilePay
        /// </summary>
        /// <param name="url">Url which MobilePay will invoke webhook on</param>
        /// <param name="events">Events which webhook should registered for</param>
        /// <returns>Response with Webhook Id</returns>
        Task<SingleWebhookResponse> RegisterWebhook(string url, ICollection<Events> events);

        /// <summary>
        /// Get Webhook information from MobilePay
        /// </summary>
        /// <param name="webhookId">Webhook Id</param>
        /// <returns>Webhook information</returns>
        Task<SingleWebhookResponse> GetWebhook(Guid webhookId);
    }
}