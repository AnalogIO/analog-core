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
            await LogMobilePayException(response);
        }

        return await response.Content.ReadAsAsync<RegisterResponse>();
    }

    public async Task<QueryResponse> GetAllWebhooksAsync()
    {
        var response = await _httpClient.GetAsync(ControllerPath);

        if (!response.IsSuccessStatusCode)
        {
            await LogMobilePayException(response);
        }

        return await response.Content.ReadAsAsync<QueryResponse>();
    }

    private async Task LogMobilePayException(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<Problem>();
        _logger.LogError("Request to [{method}] {requestUri} failed",
            response.RequestMessage!.Method,
            response.RequestMessage!.RequestUri);
        _logger.LogError("Error: {error}", problem!.Title);
        _logger.LogError("Details: {@details}", problem.ExtraDetails);
        throw new MobilePayApiException(
            503,
            $"Request to [{response.RequestMessage!.Method}] {response.RequestMessage!.RequestUri} failed for");
    }

}
