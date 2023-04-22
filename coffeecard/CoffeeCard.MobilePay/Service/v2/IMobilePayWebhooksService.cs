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

        /// <summary>
        /// Deregister Webhook at MobilePay
        /// </summary>
        /// <param name="webhookId">Existing Webhook Id</param>
        Task DeregisterWebhook(Guid webhookId);

        /// <summary>
        /// Update Webhook at MobilePay
        /// </summary>
        /// <param name="webhookId">Existing Webhook Id</param>
        /// <param name="url">Url which MobilePay will invoke webhook on</param>
        /// <param name="events">Events which webhook should registered for</param>
        /// <returns></returns>
        Task<SingleWebhookResponse> UpdateWebhook(Guid webhookId, string url, ICollection<Events> events);

        /// <summary>
        /// Get all webhooks
        /// </summary>
        /// <returns>List of Webhooks</returns>
        public Task<GetMultipleWebhooksResponse> GetAllWebhooks();
    }
}