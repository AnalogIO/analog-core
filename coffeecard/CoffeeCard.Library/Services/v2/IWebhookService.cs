using System.Threading.Tasks;

namespace CoffeeCard.Library.Services.v2
{
    public interface IWebhookService
    {
        /// <summary>
        /// Get Webhook Signature Key used for signing all Webhook requests
        /// </summary>
        Task<string> GetSignatureKey();

        /// <summary>
        /// Ensure MobilePay Webhook is registered
        /// </summary>
        Task EnsureWebhookIsRegistered();
    }
}
