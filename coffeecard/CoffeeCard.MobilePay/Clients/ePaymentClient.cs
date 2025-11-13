using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.Exception.v2;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Clients;

public class EPaymentClient(HttpClient httpClient, ILogger<EPaymentClient> logger) : IEPaymentClient
{
    private const string ControllerPath = "/epayment/v1/payments";

    public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(ControllerPath, request);

        if (!response.IsSuccessStatusCode)
        {
            await LogMobilePayException(response, request.Reference);
        }

        return await response.Content.ReadAsAsync<CreatePaymentResponse>();
    }

    public async Task<GetPaymentResponse> GetPaymentAsync(string reference)
    {
        var response = await httpClient.GetAsync($"{ControllerPath}/{reference}");

        if (!response.IsSuccessStatusCode)
        {
            await LogMobilePayException(response, reference);
        }

        return await response.Content.ReadAsAsync<GetPaymentResponse>();
    }

    public async Task<ModificationResponse> RefundPaymentAsync(
        string reference,
        RefundModificationRequest request
    )
    {
        var response = await httpClient.PostAsJsonAsync(
            $"{ControllerPath}/{reference}/refund",
            request
        );

        if (!response.IsSuccessStatusCode)
        {
            await LogMobilePayException(response, reference);
        }

        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    public async Task<ModificationResponse> CapturePaymentAsync(
        string reference,
        CaptureModificationRequest request
    )
    {
        var response = await httpClient.PostAsJsonAsync(
            $"{ControllerPath}/{reference}/capture",
            request
        );

        if (!response.IsSuccessStatusCode)
        {
            await LogMobilePayException(response, reference);
        }

        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    public async Task<ModificationResponse> CancelPaymentAsync(
        string reference,
        CancelModificationRequest request
    )
    {
        var response = await httpClient.PostAsJsonAsync(
            $"{ControllerPath}/{reference}/cancel",
            request
        );

        if (!response.IsSuccessStatusCode)
        {
            await LogMobilePayException(response, reference);
        }

        return await response.Content.ReadAsAsync<ModificationResponse>();
    }

    private async Task LogMobilePayException(HttpResponseMessage response, string reference)
    {
        var problem = await response.Content.ReadFromJsonAsync<Problem>();
        logger.LogError(
            "Request to [{method}] {requestUri} failed for reference {reference}",
            response.RequestMessage!.Method,
            response.RequestMessage!.RequestUri,
            reference
        );
        logger.LogError("Error: {error}", problem!.Title);
        logger.LogError("Details: {@details}", problem.ExtraDetails);
        throw new MobilePayApiException(
            503,
            $"Request to [{response.RequestMessage!.Method}] {response.RequestMessage!.RequestUri} failed for reference {reference}"
        );
    }
}
