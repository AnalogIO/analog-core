using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;

namespace CoffeeCard.Models.DataTransferObjects.v2.Receipts;

public class ReceiptResponse
{
    /// <summary>
    /// The list of receipts for the user.
    /// </summary>
    [Required]
    public required List<ReceiptBase> Receipts { get; set; }

    /// <summary>
    /// Used as the "from" parameter in the next request to get the next batch of receipts.
    /// Is set to the date and time of the oldest receipt in the current batch.
    /// </summary>
    [Required]
    public required string ContinueationToken { get; set; }
}

/// <summary>
/// Shared properties for all receipt types.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(PurchaseReceipt), "Purchase")]
[JsonDerivedType(typeof(VoucherReceipt), "Voucher")]
[JsonDerivedType(typeof(UsedTicketReceipt), "UsedTicket")]
public abstract class ReceiptBase
{
    [Required]
    public required string ProductName { get; set; }

    /// <summary>
    /// The date/time the receipt was issued (type-specific).
    /// Use this for sorting across all receipt types.
    /// </summary>
    [JsonIgnore]
    public abstract DateTime IssuingDate { get; }
}

public class PurchaseReceipt : ReceiptBase
{
    /// <summary>
    /// The amount of tickets issued by the purchase.
    /// </summary>
    [Required]
    public required int Quantity { get; set; }

    /// <summary>
    /// The status of the purchase.
    /// </summary>
    [Required]
    public required PurchaseStatus Status { get; set; }

    /// <summary>
    /// The date and time when the order was placed.
    /// </summary>
    [Required]
    public required DateTime OrderDate { get; set; }

    /// <summary>
    /// The total price of the purchase in kr.
    /// </summary>
    [Required]
    public required int Price { get; set; }

    /// <summary>
    /// The unique identifier of the order associated with the purchase.
    /// </summary>
    [Required]
    public required Guid OrderId { get; set; }

    [JsonIgnore]
    public override DateTime IssuingDate => OrderDate;
}

/// <summary>
/// Represents of tickets issued by a voucher.
/// </summary>
public class VoucherReceipt : ReceiptBase
{
    /// <summary>
    /// The code of the voucher.
    /// </summary>
    [Required]
    public required string Code { get; set; }

    [Required]
    public required DateTime RedeemDate { get; set; }

    /// <summary>
    /// The amount of tickets issued by the purchase.
    /// </summary>
    [Required]
    public required int Quantity { get; set; }

    [JsonIgnore]
    public override DateTime IssuingDate => RedeemDate;
}

/// <summary>
/// Represents a receipt for a ticket being used by a swipe
/// </summary>
public class UsedTicketReceipt : ReceiptBase
{
    /// <summary>
    /// The date and time when the swipe was made.
    /// </summary>
    [Required]
    public required DateTime SwipeDate { get; set; }

    [JsonIgnore]
    public override DateTime IssuingDate => SwipeDate;
}
