using System.Net.Http;
using Newtonsoft.Json;

namespace coffeecard.Helpers.MobilePay.RequestMessage
{
    public class CaptureAmountRequest : IMobilePayAPIRequestMessage
    {
        private const string EndpointUri = "reservations/merchants/{0}/orders/{1}";

        private readonly string _merchantId;
        private readonly string _orderId;

        public CaptureAmountRequest(string merchantId, string orderId)
        {
            _merchantId = merchantId;
            _orderId = orderId;
        }

        public string GetEndPointUri()
        {
            return string.Format(EndpointUri, _merchantId, _orderId);
        }

        public string GetRequestBody()
        {
            // Returns empty JSON;
            return "{}";
        }

        public HttpMethod GetHttpMethod()
        {
            return HttpMethod.Put;
        }
    }
}