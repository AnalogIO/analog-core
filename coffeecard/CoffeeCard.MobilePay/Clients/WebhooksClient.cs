using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using CoffeeCard.MobilePay.Generated.Api.WebhooksApi;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Clients;

public class WebhooksClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhooksClient> _logger;
    private const string ControllerPath = "/webhooks/v1/webhooks";

    public WebhooksClient(HttpClient httpClient, ILogger<WebhooksClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RegisterResponse> CreateWebhookAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ControllerPath, request);

        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<Problem>();
            _logger.LogError("Failed to create webhook for url {url}", request.Url);
            throw new MobilePayApiException(503, $"Failed to create webhook for url {request.Url}");
        }


        return await response.Content.ReadAsAsync<RegisterResponse>();
    }

    public async Task<QueryResponse> GetAllWebhooksAsync()
    {
        var response = await _httpClient.GetAsync(ControllerPath);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get all webhooks");
            throw new MobilePayApiException(503, "Failed to get all webhooks");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<QueryResponse>();
    }

}
