using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.AccessTokenApi;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Clients;

public class AccessTokenClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ePaymentClient> _logger;
    private const string ControllerPath = "/accesstoken";

    public AccessTokenClient(HttpClient httpClient, ILogger<ePaymentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AuthorizationTokenResponse> GetToken(string clientId, string clientSecret)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ControllerPath}/get");

        requestMessage.Headers.Add("client_id", clientId);
        requestMessage.Headers.Add("client_secret", clientSecret);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get access token");
            throw new MobilePayApiException(503, "Failed to get access token");
        }

        return await response.Content.ReadAsAsync<AuthorizationTokenResponse>();


    }
}
