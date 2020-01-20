using System.Net.Http;
using System.Threading.Tasks;
using CoffeeCard.Helpers.MobilePay.ResponseMessage;

namespace CoffeeCard.Services
{
    public interface IMobilePayService
    {
        Task<CaptureAmountResponse> CapturePayment(string orderId);

        Task<GetPaymentStatusResponse> GetPaymentStatus(string orderId);

        Task<CancelReservationResponse> CancelPaymentReservation(string orderId);
    }
}