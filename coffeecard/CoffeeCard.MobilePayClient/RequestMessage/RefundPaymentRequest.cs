using System;
using System.Net.Http;

namespace CoffeeCard.MobilePay.RequestMessage
{
    public sealed class RefundPaymentRequest : IMobilePayAPIRequestMessage
    {
        private const string EndpointUri = "merchants/{0}/orders/{1}";

        private readonly string _merchantId;
        private readonly string _orderId;

        public RefundPaymentRequest(string merchantId, string orderId)
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
            // Empty JSON
            return "{}";
        }

        public HttpMethod GetHttpMethod()
        {
            return HttpMethod.Put;
        }

        private bool Equals(RefundPaymentRequest other)
        {
            return _merchantId == other._merchantId && _orderId == other._orderId;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is RefundPaymentRequest other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_merchantId, _orderId);
        }
    }
}