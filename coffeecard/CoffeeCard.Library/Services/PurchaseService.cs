using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Common;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CoffeeCardContext _context;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(CoffeeCardContext context, ILogger<PurchaseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Purchase> RedeemVoucher(string voucherCode, IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null)
                throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            var id = int.Parse(userId.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                throw new ApiException("The user could not be found");

            var voucher = _context
                .Vouchers.Include(x => x.Product)
                .FirstOrDefault(x => x.Code.Equals(voucherCode));
            if (voucher == null)
                throw new ApiException(
                    $"Voucher '{voucherCode}' does not exist!",
                    StatusCodes.Status404NotFound
                );
            if (voucher.UserId != null)
                throw new ApiException(
                    "Voucher has already been redeemed!",
                    StatusCodes.Status409Conflict
                );

            var purchase = new Purchase
            {
                DateCreated = DateTime.UtcNow,
                NumberOfTickets = voucher.Product.NumberOfTickets,
                OrderId = (await GenerateUniqueOrderId()).ToString(),
                Price = 0,
                ProductId = voucher.Product.Id,
                ProductName = voucher.Product.Name,
                PurchasedBy = user,
                Status = PurchaseStatus.Completed,
                Type = PurchaseType.Voucher,
            };

            user.Purchases.Add(purchase);

            DeliverProductToUser(purchase, user, $"VOUCHER: {voucher.Id}");

            voucher.DateUsed = DateTime.UtcNow;
            voucher.User = user;
            voucher.Purchase = purchase;

            _context.Vouchers.Attach(voucher);
            _context.Entry(voucher).State = EntityState.Modified;
            _context.SaveChanges();

            return purchase;
        }

        private async Task<Guid> GenerateUniqueOrderId()
        {
            while (true)
            {
                var newOrderId = Guid.NewGuid();

                var orderIdAlreadyExists = await _context
                    .Purchases.Where(p => p.OrderId.Equals(newOrderId.ToString()))
                    .AnyAsync();
                if (orderIdAlreadyExists)
                    continue;

                return newOrderId;
            }
        }

        public Purchase DeliverProductToUser(Purchase purchase, User user, string transactionId)
        {
            _logger.LogInformation(
                $"Delivering product ({purchase.ProductId}) to userId: {user.Id} with orderId: {purchase.OrderId}"
            );
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            if (product == null)
                throw new ApiException(
                    $"The product with id {purchase.ProductId} could not be found!"
                );
            for (var i = 0; i < purchase.NumberOfTickets; i++)
            {
                var ticket = new Ticket { ProductId = product.Id, Purchase = purchase };
                user.Tickets.Add(ticket);
            }

            purchase.ExternalTransactionId = transactionId;

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            _logger.LogInformation(
                "Delivery of product ({productId}) to userId: {userId} with orderId: {purchaseId} succeeded!",
                purchase.ProductId,
                user.Id,
                purchase.OrderId
            );
            return purchase;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
