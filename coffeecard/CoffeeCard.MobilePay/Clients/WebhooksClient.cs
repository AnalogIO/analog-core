using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using Serilog;

namespace CoffeeCard.MobilePay.Clients;

public class WebhooksClient
{
    private readonly HttpClient _httpClient;
    private const string ControllerPath = "/v1/webhooks";

    public WebhooksClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RegisterResponse> CreateWebhookAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ControllerPath, request);

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Failed to create webhook for url {url}", request.Url);
            throw new MobilePayApiException(503, $"Failed to create webhook for url {request.Url}");
        }


        return await response.Content.ReadAsAsync<RegisterResponse>();
    }

    public async Task<QueryResponse> GetAllWebhooksAsync()
    {
        var response = await _httpClient.GetAsync(ControllerPath);

        if (!response.IsSuccessStatusCode)
        {
            Log.Error("Failed to get all webhooks");
            throw new MobilePayApiException(503, "Failed to get all webhooks");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<QueryResponse>();
    }

}
