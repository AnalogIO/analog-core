namespace CoffeeCard.Models.DataTransferObjects.v2.Receipts;

/// <summary>
/// Discriminator values that identify the concrete type of a <see cref="ReceiptListItem"/>.
/// Each value is mutually exclusive.
/// </summary>
public enum ReceiptType
{
    /// <summary>A ticket-bundle purchase made via a payment method (e.g. MobilePay).</summary>
    Purchase,

    /// <summary>Tickets issued by redeeming a voucher.</summary>
    Voucher,

    /// <summary>A single ticket consumed (swiped) by the user.</summary>
    UsedTicket,
}
