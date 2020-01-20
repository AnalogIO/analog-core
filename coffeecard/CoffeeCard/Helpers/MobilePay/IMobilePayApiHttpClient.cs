using System;
using System.Threading.Tasks;
using CoffeeCard.Helpers.MobilePay.RequestMessage;
using CoffeeCard.Helpers.MobilePay.ResponseMessage;

namespace CoffeeCard.Helpers.MobilePay
{
    public interface IMobilePayApiHttpClient : IDisposable
    {
        Task<T> SendRequest<T>(IMobilePayAPIRequestMessage requestMessage) where T: IMobilePayAPIResponse;
    }
}