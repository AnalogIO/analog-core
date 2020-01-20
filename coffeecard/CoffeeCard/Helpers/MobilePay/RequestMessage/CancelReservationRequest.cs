using System.Net.Http;

namespace CoffeeCard.Helpers.MobilePay.RequestMessage
{
    public class CancelReservationRequest : IMobilePayAPIRequestMessage
    {
        private const string EndpointUri = "reservations/merchants/{0}/orders/{1}";

        private readonly string _merchantId;
        private readonly string _orderId;

        public CancelReservationRequest(string merchantId, string orderId)
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
            return HttpMethod.Delete;
        }
    }
}