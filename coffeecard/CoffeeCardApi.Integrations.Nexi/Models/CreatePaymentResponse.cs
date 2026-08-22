namespace CoffeeCardApi.Integrations.Nexi.Models;

public class CreatePaymentResponse
{
    public required string PaymentId { get; set; }
    public required string HostedPaymentPageUrl { get; set; }
}
