using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeCard.Models.DataTransferObjects.v2.Receipts;

/// <summary>
/// Flat response wrapper returned by <c>GET /api/v2/receipts</c>.
/// Contains all receipts matching the requested filter, sorted newest-first.
/// </summary>
public class ReceiptsResponse
{
    /// <summary>
    /// The flat list of receipts for the authenticated user, sorted by
    /// <see cref="ReceiptListItem.EventDate"/> descending.
    /// </summary>
    [Required]
    public required List<ReceiptListItem> Receipts { get; set; }
}
