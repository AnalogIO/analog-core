namespace CoffeeCard.MobilePay.ResponseMessage;

public record CancelReservationResponse(string TransactionId) : IMobilePayApiResponse;