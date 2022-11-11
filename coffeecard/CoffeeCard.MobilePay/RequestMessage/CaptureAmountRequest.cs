using System;
using System.Net.Http;

namespace CoffeeCard.MobilePay.RequestMessage
{
    public sealed class CaptureAmountRequest : IMobilePayApiRequestMessage
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
            // Returns empty JSON
            return "{}";
        }

        public HttpMethod GetHttpMethod()
        {
            return HttpMethod.Put;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CaptureAmountRequest) obj);
        }

        private bool Equals(CaptureAmountRequest other)
        {
            return _merchantId == other._merchantId && _orderId == other._orderId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_merchantId, _orderId);
        }
    }
}