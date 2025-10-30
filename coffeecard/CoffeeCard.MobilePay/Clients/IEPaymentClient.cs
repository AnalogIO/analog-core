using System.Threading.Tasks;
using CoffeeCard.MobilePay.Generated.Api.ePaymentApi;

namespace CoffeeCard.MobilePay.Clients;

public interface IEPaymentClient
{
    public Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);

    public Task<GetPaymentResponse> GetPaymentAsync(string reference);

    public Task<ModificationResponse> RefundPaymentAsync(
        string reference,
        RefundModificationRequest request
    );

    public Task<ModificationResponse> CapturePaymentAsync(
        string reference,
        CaptureModificationRequest request
    );

    public Task<ModificationResponse> CancelPaymentAsync(
        string reference,
        CancelModificationRequest request
    );
}
