using System.Threading.Tasks;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;

namespace CoffeeCard.MobilePay.Clients;

public interface IWebhooksClient
{
    Task<RegisterResponse> CreateWebhookAsync(RegisterRequest request);
    Task<QueryResponse> GetAllWebhooksAsync();
}
