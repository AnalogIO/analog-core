using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Clients;

public class ePaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ePaymentClient> _logger;
    private const string ControllerPath = "/v1/payments";

    public ePaymentClient(HttpClient httpClient, ILogger<ePaymentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var respose = await _httpClient.PostAsJsonAsync(ControllerPath, request);

        if (!respose.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to create payment for reference {reference}", request.Reference); // TODO: Request details?
            throw new MobilePayApiException(503, $"Failed to create payment for reference {request.Reference}");
        }
        
        return await respose.Content.ReadAsAsync<CreatePaymentResponse>();
    }

    public async Task<GetPaymentResponse> GetPaymentAsync(string reference)
    {
        var response = await _httpClient.GetAsync($"{ControllerPath}/{reference}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to get payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<GetPaymentResponse>();
    }

    public async Task<ModificationResponse> RefundPaymentAsync(string reference, RefundModificationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ControllerPath}/{reference}/refund", request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to refund payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to refund payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    public async Task<ModificationResponse> CapturePaymentAsync(string reference, CaptureModificationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ControllerPath}/{reference}/capture", request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to capture payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to capture payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    public async Task<ModificationResponse> CancelPaymentAsync(string reference, CancelModificationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ControllerPath}/{reference}/cancel", request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to cancel payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to cancel payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<ModificationResponse>();
    }
    
    
}