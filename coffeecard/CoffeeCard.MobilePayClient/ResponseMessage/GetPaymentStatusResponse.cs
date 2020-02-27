using System;

namespace CoffeeCard.MobilePay.ResponseMessage
{
    public class GetPaymentStatusResponse : IMobilePayAPIResponse
    {
        public PaymentStatus LatestPaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public double OriginalAmount { get; set; }

        protected bool Equals(GetPaymentStatusResponse other)
        {
            return LatestPaymentStatus == other.LatestPaymentStatus && TransactionId == other.TransactionId &&
                   OriginalAmount.Equals(other.OriginalAmount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GetPaymentStatusResponse) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) LatestPaymentStatus, TransactionId, OriginalAmount);
        }
    }
}