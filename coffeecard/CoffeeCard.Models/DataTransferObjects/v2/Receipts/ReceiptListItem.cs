using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Receipts;

/// <summary>
/// A single receipt entry in the flat receipt list.
/// All fields are present on every item; nullable fields are null when not applicable for the
/// receipt <see cref="Type"/>.
/// </summary>
public class ReceiptListItem
{
    /// <summary>
    /// Composite string identifier in the format <c>"TypeName:EntityId"</c>,
    /// e.g. <c>"Purchase:123"</c>, <c>"Voucher:456"</c>, or <c>"UsedTicket:789"</c>.
    /// The numeric part is the entity's database primary key.
    /// </summary>
    [Required]
    public required string Id { get; set; }

    /// <summary>
    /// The discriminator type of this receipt entry.
    /// </summary>
    [Required]
    public required ReceiptType Type { get; set; }

    /// <summary>
    /// The canonical event date used for sorting across all receipt types.
    /// For purchases and vouchers this is the order/redeem date; for used tickets it is the swipe date.
    /// Expressed in UTC.
    /// </summary>
    [Required]
    public required DateTime EventDate { get; set; }

    /// <summary>
    /// Server-assembled, human-readable summary of this receipt entry, e.g.
    /// <c>"Purchased 10x Filter"</c>, <c>"Redeemed 5x Filter tickets"</c>, or <c>"Swiped a Filter Coffee"</c>.
    /// </summary>
    [Required]
    public required string Title { get; set; }

    /// <summary>
    /// Number of tickets involved in this receipt.
    /// Set for <see cref="ReceiptType.Purchase"/> and <see cref="ReceiptType.Voucher"/>;
    /// <c>null</c> for <see cref="ReceiptType.UsedTicket"/>.
    /// </summary>
    public int? Amount { get; set; }

    /// <summary>
    /// Total price paid in Danish kroner (DKK).
    /// Only set for <see cref="ReceiptType.Purchase"/>; <c>null</c> for all other types.
    /// </summary>
    public int? PriceDKK { get; set; }

    /// <summary>
    /// The name of the product or ticket type, e.g. <c>"Filter"</c>.
    /// </summary>
    [Required]
    public required string TicketName { get; set; }

    /// <summary>
    /// The name of the drink or menu item the ticket was used on.
    /// Only set for <see cref="ReceiptType.UsedTicket"/> items where a menu item was recorded;
    /// <c>null</c> for all other types and for used tickets with no associated menu item.
    /// </summary>
    public string? DrinkName { get; set; }
}
