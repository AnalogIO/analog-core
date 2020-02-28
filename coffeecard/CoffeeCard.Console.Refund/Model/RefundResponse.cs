using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeCard.Console.Refund.Model
{
    public class RefundResponse
    {
        public Status Status { get; set; }
        public string OrderId { get; set; }
        public string? OriginalTransactionId { get; set; }
        public string? RefundTransactionId { get; set; }
        public double? Remainder { get; set; }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(OrderId)}: {OrderId}, {nameof(OriginalTransactionId)}: {OriginalTransactionId}, {nameof(RefundTransactionId)}: {RefundTransactionId}, {nameof(Remainder)}: {Remainder}";
        }
    }

    public enum Status { Success, Failed }
}
