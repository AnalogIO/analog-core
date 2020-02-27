using System.Net.Http;

namespace CoffeeCard.MobilePay.RequestMessage
{
    public interface IMobilePayAPIRequestMessage
    {
        string GetEndPointUri();
        string GetRequestBody();
        HttpMethod GetHttpMethod();
    }
}