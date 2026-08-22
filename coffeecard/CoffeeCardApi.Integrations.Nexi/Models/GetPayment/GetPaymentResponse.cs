namespace CoffeeCardApi.Integrations.Nexi.Models.GetPayment;

public class GetPaymentResponse
{
    public Payment Payment { get; set; }
}

public class BillingAddress
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string ReceiverLine { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
}

public class CardDetails
{
    public string MaskedPan { get; set; }
    public string ExpiryDate { get; set; }
}

public class Charge
{
    public string ChargeId { get; set; }
    public int? Amount { get; set; }
    public int? SurchargeAmount { get; set; }
    public DateTime? Created { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

public class Checkout
{
    public string Url { get; set; }
    public string CancelUrl { get; set; }
}

public class Company
{
    public string MerchantReference { get; set; }
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public ContactDetails ContactDetails { get; set; }
}

public class Consumer
{
    public ShippingAddress ShippingAddress { get; set; }
    public Company Company { get; set; }
    public PrivatePerson PrivatePerson { get; set; }
    public BillingAddress BillingAddress { get; set; }
}

public class ContactDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
}

public class InvoiceDetails
{
    public string InvoiceNumber { get; set; }
}

public class OrderDetails
{
    public int? Amount { get; set; }
    public string Currency { get; set; }
    public string Reference { get; set; }
}

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

public class Payment
{
    public string PaymentId { get; set; }
    public Summary Summary { get; set; }
    public Consumer Consumer { get; set; }
    public PaymentDetails PaymentDetails { get; set; }
    public OrderDetails OrderDetails { get; set; }
    public Checkout Checkout { get; set; }
    public DateTime? Created { get; set; }
    public List<Refund> Refunds { get; set; }
    public List<Charge> Charges { get; set; }
    public DateTime? Terminated { get; set; }
    public Subscription Subscription { get; set; }
    public UnscheduledSubscription UnscheduledSubscription { get; set; }
    public string MyReference { get; set; }
    public string PaymentAccountReference { get; set; }
}

public class PaymentDetails
{
    public string PaymentType { get; set; }
    public string PaymentMethod { get; set; }
    public InvoiceDetails InvoiceDetails { get; set; }
    public CardDetails CardDetails { get; set; }
}

public class PhoneNumber
{
    public string Prefix { get; set; }
    public string Number { get; set; }
}

public class PrivatePerson
{
    public string MerchantReference { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
}

public class Refund
{
    public string RefundId { get; set; }
    public int? Amount { get; set; }
    public int? SurchargeAmount { get; set; }
    public string State { get; set; }
    public DateTime? LastUpdated { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

public class ShippingAddress
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string ReceiverLine { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
}

public class Subscription
{
    public string Id { get; set; }
}

public class Summary
{
    public int? ReservedAmount { get; set; }
    public int? ReservedSurchargeAmount { get; set; }
    public int? ChargedAmount { get; set; }
    public int? ChargedSurchargeAmount { get; set; }
    public int? RefundedAmount { get; set; }
    public int? RefundedSurchargeAmount { get; set; }
    public int? CancelledAmount { get; set; }
    public int? CancelledSurchargeAmount { get; set; }
}

public class UnscheduledSubscription
{
    public string UnscheduledSubscriptionId { get; set; }
}
