using System.Net.Http;
using System.Threading.Tasks;

namespace CoffeeCard.Services
{
    public interface IMobilePayService
    {
        Task<HttpResponseMessage> CapturePayment(string orderId);

        Task<HttpResponseMessage> GetPaymentStatus(string orderId);

        Task<HttpResponseMessage> CancelPaymentReservation(string orderId);
    }
}