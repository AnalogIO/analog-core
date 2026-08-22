namespace CoffeeCardApi.Integrations.Nexi.Models;

internal class CreatePaymentRequest
{
    public required Order Order { get; set; }
    public required Checkout Checkout { get; set; }
    public string? MerchantNumber { get; set; }
    public Notifications? Notifications { get; set; }
    public List<PaymentMethodsConfiguration>? PaymentMethodsConfiguration { get; set; }
    public List<PaymentMethod>? PaymentMethods { get; set; }
    public string? MyReference { get; set; }
}

internal class Appearance
{
    public DisplayOptions DisplayOptions { get; set; }
    public TextOptions TextOptions { get; set; }
}

internal class BillingAddress
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}

internal class Checkout
{
    public string? Url { get; set; }
    public string? IntegrationType { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
    public Consumer? Consumer { get; set; }
    public required string? TermsUrl { get; set; }
    public string? MerchantTermsUrl { get; set; }
    public List<ShippingCountry>? ShippingCountries { get; set; }
    public Shipping? Shipping { get; set; }
    public ConsumerType? ConsumerType { get; set; }
    public bool? Charge { get; set; }
    public bool? PublicDevice { get; set; }
    public bool? MerchantHandlesConsumerData { get; set; }
    public Appearance? Appearance { get; set; }
    public string? CountryCode { get; set; }
}

internal class Company
{
    public string Name { get; set; }
    public Contact Contact { get; set; }
}

internal class Consumer
{
    public string Reference { get; set; }
    public string Email { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public BillingAddress BillingAddress { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public PrivatePerson PrivatePerson { get; set; }
    public Company Company { get; set; }
}

internal class ConsumerType
{
    public string Default { get; set; }
    public List<string> SupportedTypes { get; set; }
}

internal class Contact
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

internal class Country
{
    public string CountryCode { get; set; }
}

internal class DisplayOptions
{
    public bool? ShowMerchantName { get; set; }
    public bool? ShowOrderSummary { get; set; }
}

internal class Fee
{
    public string Reference { get; set; }
    public string Name { get; set; }
    public double? Quantity { get; set; }
    public string Unit { get; set; }
    public int? UnitPrice { get; set; }
    public int? TaxRate { get; set; }
    public int? TaxAmount { get; set; }
    public int? GrossTotalAmount { get; set; }
    public int? NetTotalAmount { get; set; }
    public string ImageUrl { get; set; }
}

internal class Item
{
    public required string Reference { get; set; }
    public required string Name { get; set; }
    public required double Quantity { get; set; }
    public required string Unit { get; set; }
    public required int UnitPrice { get; set; }
    public int? TaxRate { get; set; }
    public int? TaxAmount { get; set; }
    public required int GrossTotalAmount { get; set; }
    public required int NetTotalAmount { get; set; }
    public string ImageUrl { get; set; }
}

internal class Notifications
{
    public List<WebHook> WebHooks { get; set; }
}

internal class Order
{
    public required List<Item> Items { get; set; }
    public required int Amount { get; set; }
    public required string Currency { get; set; }
    public string? Reference { get; set; }
}

internal class PaymentMethod
{
    public string Name { get; set; }
    public Fee Fee { get; set; }
}

internal class PaymentMethodsConfiguration
{
    public string Name { get; set; }
    public bool? Enabled { get; set; }
}

internal class PayPal
{
    public string OrderReference { get; set; }
}

internal class PhoneNumber
{
    public string Prefix { get; set; }
    public string Number { get; set; }
}

internal class PrivatePerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

internal class Riverty
{
    public string OrderReference { get; set; }
}

internal class Shipping
{
    public List<Country> Countries { get; set; }
    public bool? MerchantHandlesShippingCost { get; set; }
    public bool? EnableBillingAddress { get; set; }
}

internal class ShippingAddress
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}

internal class ShippingCountry
{
    public string CountryCode { get; set; }
}

internal class TextOptions
{
    public string CompletePaymentButtonText { get; set; }
}

internal class WebHook
{
    public string EventName { get; set; }
    public string Url { get; set; }
    public string Authorization { get; set; }
    public object Headers { get; set; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class OrderItem
{
    public string Reference { get; set; }
    public string Name { get; set; }
    public double? Quantity { get; set; }
    public string Unit { get; set; }
    public int? UnitPrice { get; set; }
    public int? TaxRate { get; set; }
    public int? TaxAmount { get; set; }
    public int? GrossTotalAmount { get; set; }
    public int? NetTotalAmount { get; set; }
    public string ImageUrl { get; set; }
}

public class ChargeRequest
{
    public int? Amount { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public ShippingCharge Shipping { get; set; }
    public bool? FinalCharge { get; set; }
    public string MyReference { get; set; }
    public string PaymentMethodReference { get; set; }
}

public class ShippingCharge
{
    public string TrackingNumber { get; set; }
    public string Provider { get; set; }
}
