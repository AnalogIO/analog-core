using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeCard.Console.Refund.Model
{
    public class RefundResponse
    {
        public Status Status { get; set; }
        public string OrderId { get; set; }

        public override string ToString()
        {
            return $"{nameof(Status)}: {Status}, {nameof(OrderId)}: {OrderId}";
        }
    }

    public enum Status { Success, Failed }
}
