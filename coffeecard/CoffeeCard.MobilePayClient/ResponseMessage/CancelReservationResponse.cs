﻿namespace CoffeeCard.MobilePay.ResponseMessage
{
    public class CancelReservationResponse : IMobilePayAPIResponse
    {
        public string TransactionId { get; set; }
    }
}