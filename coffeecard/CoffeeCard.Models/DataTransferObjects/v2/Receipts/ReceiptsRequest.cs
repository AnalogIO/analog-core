using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Receipts;

/// <summary>
/// A paginated request for receipts based on using a continuation token
/// </summary>
public class ReceiptsRequest
{
    [Required]
    public required ReceiptType Type { get; set; }

    [Required]
    public int BatchSize { get; set; } = 20;

    public required string? ContinueationToken { get; set; }
}

/// <summary>
/// The types of receipts that can be requested. Multiple types can be combined.
/// </summary>
[Flags]
public enum ReceiptType
{
    Purchase = 1,
    Voucher = 2,
    UsedTicket = 4,
    All = Purchase | Voucher | UsedTicket,
}
