using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;

namespace CoffeeCard.MobilePay.Clients;

public class WebhooksClient
{
    private readonly HttpClient _httpClient;
    private const string ControllerPath = "/v1/webhooks";

    public WebhooksClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public virtual async Task<RegisterResponse> CreateWebhookAsync(RegisterRequest body)
    {
        var response = await _httpClient.PostAsJsonAsync(ControllerPath, body);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<RegisterResponse>();
    }

}