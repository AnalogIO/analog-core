using System;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.ResponseMessage;

namespace CoffeeCard.MobilePay.Service
{
    public interface IMobilePayService : IDisposable
    {
        Task<CaptureAmountResponse> CapturePayment(string orderId);
        Task<GetPaymentStatusResponse> GetPaymentStatus(string orderId);
        Task<CancelReservationResponse> CancelPaymentReservation(string orderId);
        Task<RefundPaymentResponse> RefundPayment(string orderId);
    }
}