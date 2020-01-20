using coffeecard.Helpers.MobilePay.ResponseMessage;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoffeeCard.Services
{
    public interface IMobilePayService
    {
        Task<CaptureAmountResponse> CapturePayment(string orderId);

        Task<GetPaymentStatusResponse> GetPaymentStatus(string orderId);

        Task<CancelReservationResponse> CancelPaymentReservation(string orderId);
    }
}