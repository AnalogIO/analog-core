using System.Net.Http;

namespace coffeecard.Helpers.MobilePay.RequestMessage
{
    public interface IMobilePayAPIRequestMessage
    {
        string GetEndPointUri();
        string GetRequestBody();
        HttpMethod GetHttpMethod();
    }
}