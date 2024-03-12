using CoffeeCard.Common;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CoffeeCardContext _context;

        public PurchaseService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<Purchase> RedeemVoucher(string voucherCode, IEnumerable<Claim> claims)
        {
            Claim userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if (userId == null) throw new ApiException("The token is invalid!", StatusCodes.Status401Unauthorized);
            int id = int.Parse(userId.Value);

            User user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) throw new ApiException("The user could not be found");

            Voucher voucher = _context.Vouchers.Include(x => x.Product).FirstOrDefault(x => x.Code.Equals(voucherCode));
            if (voucher == null) throw new ApiException($"Voucher '{voucherCode}' does not exist!", StatusCodes.Status404NotFound);
            if (voucher.UserId != null) throw new ApiException("Voucher has already been redeemed!", StatusCodes.Status409Conflict);

            Purchase purchase = new Purchase
            {
                DateCreated = DateTime.UtcNow,
                NumberOfTickets = voucher.Product.NumberOfTickets,
                OrderId = (await GenerateUniqueOrderId()).ToString(),
                Price = 0,
                ProductId = voucher.Product.Id,
                ProductName = voucher.Product.Name,
                PurchasedBy = user,
                Status = PurchaseStatus.Completed,
                Type = PurchaseType.Voucher
            };

            user.Purchases.Add(purchase);

            _ = DeliverProductToUser(purchase, user, $"VOUCHER: {voucher.Id}");

            voucher.DateUsed = DateTime.UtcNow;
            voucher.User = user;
            voucher.Purchase = purchase;

            _ = _context.Vouchers.Attach(voucher);
            _context.Entry(voucher).State = EntityState.Modified;
            _ = _context.SaveChanges();

            return purchase;
        }

        private async Task<Guid> GenerateUniqueOrderId()
        {
            while (true)
            {
                Guid newOrderId = Guid.NewGuid();

                bool orderIdAlreadyExists =
                    await _context.Purchases.Where(p => p.OrderId.Equals(newOrderId.ToString())).AnyAsync();
                if (orderIdAlreadyExists) continue;

                return newOrderId;
            }
        }

        public Purchase DeliverProductToUser(Purchase purchase, User user, string transactionId)
        {
            Log.Information(
                $"Delivering product ({purchase.ProductId}) to userId: {user.Id} with orderId: {purchase.OrderId}");
            Product product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            if (product == null)
                throw new ApiException($"The product with id {purchase.ProductId} could not be found!");
            for (int i = 0; i < purchase.NumberOfTickets; i++)
            {
                Ticket ticket = new Ticket { ProductId = product.Id, Purchase = purchase };
                user.Tickets.Add(ticket);
            }

            purchase.ExternalTransactionId = transactionId;

            _ = _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            _ = _context.SaveChanges();

            Log.Information(
                $"Delivery of product ({purchase.ProductId}) to userId: {user.Id} with orderId: {purchase.OrderId} succeeded!");
            return purchase;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}