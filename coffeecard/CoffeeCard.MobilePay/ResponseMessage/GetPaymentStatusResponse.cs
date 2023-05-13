using System;

namespace CoffeeCard.MobilePay.ResponseMessage;

public record GetPaymentStatusResponse(PaymentStatus LatestPaymentStatus, 
    string TransactionId, 
    double OriginalAmount) : IMobilePayApiResponse;
