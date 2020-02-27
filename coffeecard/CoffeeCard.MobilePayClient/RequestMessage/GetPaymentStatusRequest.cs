using System;
using System.Net.Http;

namespace CoffeeCard.MobilePay.RequestMessage
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

        private bool Equals(GetPaymentStatusRequest other)
        {
            return _merchantId == other._merchantId && _orderId == other._orderId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GetPaymentStatusRequest) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_merchantId, _orderId);
        }
    }
}