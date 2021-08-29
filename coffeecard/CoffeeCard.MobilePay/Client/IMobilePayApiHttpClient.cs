using System;
using System.Threading.Tasks;
using CoffeeCard.MobilePay.RequestMessage;
using CoffeeCard.MobilePay.ResponseMessage;

namespace CoffeeCard.MobilePay.Client
{
    public interface IMobilePayApiHttpClient : IDisposable
    {
        Task<T> SendRequest<T>(IMobilePayApiRequestMessage requestMessage) where T : IMobilePayApiResponse;
    }
}