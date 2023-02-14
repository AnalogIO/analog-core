using System.Net.Http;

namespace CoffeeCard.MobilePay.RequestMessage
{
    public interface IMobilePayApiRequestMessage
    {
        string GetEndPointUri();
        string GetRequestBody();
        HttpMethod GetHttpMethod();
    }
}