using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Receipts;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services.v2;

/// <summary>
/// Implementation of <see cref="IReceiptService"/> that queries the database for receipts
/// and returns a flat, merged, sorted list.
/// </summary>
public class ReceiptService : IReceiptService
{
    private readonly ILogger<ReceiptService> _logger;
    private readonly CoffeeCardContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="ReceiptService"/>.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="context">EF Core database context.</param>
    public ReceiptService(ILogger<ReceiptService> logger, CoffeeCardContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ReceiptsResponse> GetReceipts(int userId)
    {
        var all = new List<ReceiptListItem>();

        var purchases = await _context
            .Purchases.AsNoTracking()
            .Where(p =>
                p.PurchasedById == userId
                && p.Type != PurchaseType.Voucher
                && p.Type != PurchaseType.Free
            )
            .Select(p => new ReceiptListItem
            {
                Id = "Purchase:" + p.Id,
                Type = ReceiptType.Purchase,
                EventDate = p.DateCreated,
                Title = "Purchased " + p.NumberOfTickets + "x " + p.ProductName,
                Amount = p.NumberOfTickets,
                PriceDKK = p.Price,
                TicketName = p.ProductName,
                DrinkName = null,
            })
            .ToListAsync();

        all.AddRange(purchases);

        var vouchers = await _context
            .Purchases.AsNoTracking()
            .Where(p => p.PurchasedById == userId && p.Type == PurchaseType.Voucher)
            .Select(p => new ReceiptListItem
            {
                Id = "Voucher:" + p.Id,
                Type = ReceiptType.Voucher,
                EventDate = p.DateCreated,
                Title = "Redeemed " + p.NumberOfTickets + "x " + p.ProductName + " tickets",
                Amount = p.NumberOfTickets,
                PriceDKK = null,
                TicketName = p.ProductName,
                DrinkName = null,
            })
            .ToListAsync();

        all.AddRange(vouchers);

        var usedTickets = await _context
            .Tickets.AsNoTracking()
            .Where(t => t.OwnerId == userId && t.DateUsed != null)
            .Select(t => new ReceiptListItem
            {
                Id = "UsedTicket:" + t.Id,
                Type = ReceiptType.UsedTicket,
                EventDate = t.DateUsed!.Value,
                Title =
                    t.UsedOnMenuItem != null
                        ? "Swiped a " + t.UsedOnMenuItem.Name
                        : "Swiped a " + t.Purchase.ProductName + " ticket",
                Amount = null,
                PriceDKK = null,
                TicketName = t.Purchase.ProductName,
                DrinkName = t.UsedOnMenuItem != null ? t.UsedOnMenuItem.Name : null,
            })
            .ToListAsync();

        all.AddRange(usedTickets);

        var sorted = all.OrderByDescending(r => r.EventDate)
            .ThenByDescending(r => ParseIdNumber(r.Id))
            .ToList();

        return new ReceiptsResponse { Receipts = sorted };
    }

    /// <summary>
    /// Parses the numeric entity-ID component from a composite receipt identifier such as
    /// <c>"Purchase:123"</c> and returns it as an <see cref="int"/>.
    /// </summary>
    /// <param name="id">The composite ID string.</param>
    /// <returns>The numeric database primary key embedded in <paramref name="id"/>.</returns>
    private static int ParseIdNumber(string id)
    {
        var parts = id.Split(':');
        return int.Parse(parts[1]);
    }
}
