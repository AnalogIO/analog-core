namespace CoffeeCard.MobilePay.ResponseMessage;

public record RefundPaymentResponse(string OriginalTransactionId, string? TransactionId = default, double? Remainder = default) : IMobilePayApiResponse;