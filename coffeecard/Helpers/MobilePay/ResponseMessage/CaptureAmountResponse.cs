﻿namespace CoffeeCard.Helpers.MobilePay.ResponseMessage
{
    public class CaptureAmountResponse : IMobilePayAPIResponse
    {
        public string TransactionId { get; set; }
    }
}
