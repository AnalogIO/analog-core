using System;

namespace CoffeeCard.Helpers.MobilePay.ResponseMessage
{
    public class GetPaymentStatusResponse : IMobilePayAPIResponse
    {
        protected bool Equals(GetPaymentStatusResponse other)
        {
            return LatestPaymentStatus == other.LatestPaymentStatus && TransactionId == other.TransactionId && OriginalAmount.Equals(other.OriginalAmount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GetPaymentStatusResponse) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) LatestPaymentStatus, TransactionId, OriginalAmount);
        }

        public PaymentStatus LatestPaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public double OriginalAmount { get; set; }
    }
}
