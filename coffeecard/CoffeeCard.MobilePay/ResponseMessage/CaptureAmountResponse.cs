namespace CoffeeCard.MobilePay.ResponseMessage;

public record CaptureAmountResponse(string TransactionId) : IMobilePayApiResponse;