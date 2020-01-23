using System.Net.Http;

namespace CoffeeCard.Helpers.MobilePay.RequestMessage
{
    public interface IMobilePayAPIRequestMessage
    {
        string GetEndPointUri();
        string GetRequestBody();
        HttpMethod GetHttpMethod();
    }
}