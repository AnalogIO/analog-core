using System.Net.Http;

namespace coffeecard.Helpers.MobilePay.RequestMessage
{
    public class GetPaymentStatusRequest : IMobilePayAPIRequestMessage
    {
        private const string EndpointUri = "merchants/{0}/orders/{1}";

        private readonly string _merchantId;
        private readonly string _orderId;

        public GetPaymentStatusRequest(string merchantId, string orderId)
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
            return "";
        }

        public HttpMethod GetHttpMethod()
        {
            return HttpMethod.Get;
        }
    }
}