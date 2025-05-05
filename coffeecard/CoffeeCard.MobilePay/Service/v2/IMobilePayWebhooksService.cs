using System;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.MobilePay;

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
        Task<RegisterWebhookResponse> RegisterWebhook(string url);

        /// <summary>
        /// Get Webhook information from MobilePay
        /// </summary>
        /// <param name="webhookId">Webhook Id</param>
        /// <returns>Webhook information</returns>
        Task<GetWebhookResponse> GetWebhook(Guid webhookId);

        /// <summary>
        /// Get All Webhooks from MobilePay
        /// </summary>
        /// <returns>Webhooks information</returns>
        Task<GetAllWebhooksResponse> GetAllWebhooks();
    }
}
