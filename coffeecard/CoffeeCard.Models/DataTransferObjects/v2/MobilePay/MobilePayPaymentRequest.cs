using System;

namespace CoffeeCard.Models.DataTransferObjects.v2.MobilePay
{
    public class MobilePayPaymentRequest
    {
        public int Amount { get; set; }
        
        public Guid OrderId { get; set; }
        
        public string Description { get; set; }
    }
}