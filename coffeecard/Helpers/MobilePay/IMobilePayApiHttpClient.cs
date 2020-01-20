using System;
using System.Threading.Tasks;
using coffeecard.Helpers.MobilePay.RequestMessage;
using coffeecard.Helpers.MobilePay.ResponseMessage;

namespace coffeecard.Helpers.MobilePay
{
    public interface IMobilePayApiHttpClient : IDisposable
    {
        Task<T> SendRequest<T>(IMobilePayAPIRequestMessage requestMessage) where T: IMobilePayAPIResponse;
    }
}