namespace CoffeeCard.Models.DataTransferObjects.v2.Receipts;

/// <summary>
/// Filter values for the <c>type</c> query parameter on <c>GET /api/v2/receipts</c>.
/// <see cref="All"/> (the default) returns every receipt type in a single merged list.
/// </summary>
public enum ReceiptTypeFilter
{
    /// <summary>Return all receipt types (purchases, vouchers, and used tickets).</summary>
    All = 0,

    /// <summary>Return only purchase receipts.</summary>
    Purchase,

    /// <summary>Return only voucher receipts.</summary>
    Voucher,

    /// <summary>Return only used-ticket receipts.</summary>
    UsedTicket,
}

/// <summary>
/// Query-string parameters for <c>GET /api/v2/receipts</c>.
/// </summary>
public class ReceiptsRequest
{
    /// <summary>
    /// The receipt type to include in the response.
    /// Defaults to <see cref="ReceiptTypeFilter.All"/>, which returns every type merged into a
    /// single list sorted by event date descending.
    /// </summary>
    public ReceiptTypeFilter Type { get; set; } = ReceiptTypeFilter.All;
}
