using System.Text.Json;
using System.Text.Json.Serialization;
using CoffeeCardApi.Integrations.Nexi.Models;
using CoffeeCardApi.Integrations.Nexi.Models.GetPayment;

namespace CoffeeCardApi.Integrations.Nexi;

internal class NexiPaymentApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

    public NexiPaymentApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request)
    {
        const string endpoint = "/v1/payments";
        var response = await _httpClient.PostAsJsonAsync(endpoint, request);

        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<CreatePaymentResponse>(
                await response.Content.ReadAsStringAsync(),
                _jsonSerializerOptions
            ) ?? throw new NullReferenceException();
    }

    public async Task<GetPaymentResponse> GetPaymentInfo(string paymentId)
    {
        const string endpoint = "/v1/payments/{paymentId}";

        var response = await _httpClient.GetAsync($"{endpoint}/{paymentId}");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<GetPaymentResponse>(
                await response.Content.ReadAsStringAsync(),
                _jsonSerializerOptions
            ) ?? throw new NullReferenceException();
    }
}
