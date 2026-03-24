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

public interface IReceiptService
{
    Task<ReceiptResponse> GetReceipts(
        DateTime from,
        int userId,
        ReceiptType type,
        int batchSize = 20
    );
}

public class ReceiptService : IReceiptService
{
    private readonly ILogger<ReceiptService> _logger;
    private readonly CoffeeCardContext _context;

    public ReceiptService(ILogger<ReceiptService> logger, CoffeeCardContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ReceiptResponse> GetReceipts(
        DateTime from,
        int batchSize,
        ReceiptType type,
        int userId
    )
    {
        var ticketReceipts = !type.HasFlag(ReceiptType.UsedTicket)
            ? []
            : await _context
                .Tickets.Where(t => t.OwnerId == userId)
                .Where(ticket => ticket.DateUsed >= from)
                .Where(ticket => ticket.DateUsed != null)
                .OrderByDescending(ticket => ticket.DateCreated)
                .Take(batchSize)
                .Select(t => new UsedTicketReceipt
                {
                    ProductName = t.Purchase.ProductName,
                    SwipeDate = t.DateUsed.Value,
                })
                .ToListAsync();

        var voucherReceipts = !type.HasFlag(ReceiptType.Voucher)
            ? []
            : await _context
                .Purchases.Where(p => p.PurchasedById == userId)
                .Where(purchase => purchase.DateCreated >= from)
                .Where(p => p.Type == PurchaseType.Voucher)
                .OrderByDescending(purchase => purchase.DateCreated)
                .Take(batchSize)
                .Select(p => new VoucherReceipt
                {
                    Code = p.Voucher.Code,
                    Quantity = p.NumberOfTickets,
                    ProductName = p.ProductName,
                    RedeemDate = p.DateCreated,
                })
                .ToListAsync();

        var purchaseReceipts = !type.HasFlag(ReceiptType.Purchase)
            ? []
            : await _context
                .Purchases.Where(p => p.PurchasedById == userId)
                .Where(purchase => purchase.DateCreated >= from)
                .Where(p => p.Type != PurchaseType.Voucher)
                .OrderByDescending(purchase => purchase.DateCreated)
                .Take(batchSize)
                .Select(p => new PurchaseReceipt
                {
                    OrderId = Guid.Parse(p.OrderId),
                    ProductName = p.ProductName,
                    Quantity = p.NumberOfTickets,
                    Status = p.Status,
                    OrderDate = p.DateCreated,
                    Price = p.Price,
                })
                .ToListAsync();

        List<ReceiptBase> combinedReceipts =
        [
            .. purchaseReceipts,
            .. ticketReceipts,
            .. voucherReceipts,
        ];
        var receiptList = combinedReceipts
            .OrderByDescending(r => r.IssuingDate)
            .Take(batchSize)
            .ToList();

        // As they are ordered by date, the last index will represent the continuation token
        var continuationToken = receiptList[^1].IssuingDate;
        var encodedToken = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(continuationToken.ToString("O"))
        );
        return new ReceiptResponse
        {
            Receipts = receiptList,
            // As they are ordered by date, the last index will represent the continuation token
            ContinueationToken = encodedToken,
        };
    }
}
