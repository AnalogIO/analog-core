using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Clients;

public class ePaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ePaymentClient> _logger;
    private const string ControllerPath = "/epayment/v1/payments";

    public ePaymentClient(HttpClient httpClient, ILogger<ePaymentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ControllerPath, request);
        // TODO: For some reason this does not work with custom httprequest (to attach header)
        // Currently just attached using default header
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<Problem>();
            // TODO: Handle errors nicely. Details are stored in problem.ExtraDetails
            _logger.LogError("Failed to create payment for reference {reference}: {error}", request.Reference, problem.Title); // TODO: Request details?
            throw new MobilePayApiException(503, $"Failed to create payment for reference {request.Reference}");
        }
        
        return await response.Content.ReadAsAsync<CreatePaymentResponse>();
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
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ControllerPath}/{reference}/refund");
        requestMessage.Content = JsonContent.Create(request);
        requestMessage.Headers.Add("Idempotency-Key", reference);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to refund payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to refund payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    public async Task<ModificationResponse> CapturePaymentAsync(string reference, CaptureModificationRequest request)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ControllerPath}/{reference}/capture");
        requestMessage.Content = JsonContent.Create(request);
        requestMessage.Headers.Add("Idempotency-Key", reference);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to capture payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to capture payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    public async Task<ModificationResponse> CancelPaymentAsync(string reference, CancelModificationRequest request)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ControllerPath}/{reference}/cancel");
        requestMessage.Content = JsonContent.Create(request);
        requestMessage.Headers.Add("Idempotency-Key", reference);

        var response = await _httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to cancel payment for reference {reference}", reference);
            throw new MobilePayApiException(503, $"Failed to cancel payment for reference {reference}");
        }
        
        return await response.Content.ReadAsAsync<ModificationResponse>();
    }
    
    
}