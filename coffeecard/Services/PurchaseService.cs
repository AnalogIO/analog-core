using coffeecard.Helpers;
using coffeecard.Models.DataTransferObjects.Purchase;
using Coffeecard.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly CoffeecardContext _context;

        public PurchaseService(CoffeecardContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var purchase = _context.Purchases.Find(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public Purchase Read(string orderId)
        {
            return _context.Purchases.Where(x => x.OrderId == orderId).FirstOrDefault();
        }

        public IEnumerable<Purchase> Read(DateTime from, DateTime to)
        {
            return _context.Purchases.Where(x => x.DateCreated >= from && x.DateCreated <= to).ToList();
        }

        public IEnumerable<Purchase> Read(DateTime from)
        {
            return _context.Purchases.Where(x => x.DateCreated >= from).ToList();
        }

        public int Update(Purchase purchase)
        {
            _context.Purchases.Attach(purchase);
            return _context.SaveChanges();
        }

        public void Update()
        {
            var purchases = _context.Purchases.Where(x => x.PurchasedBy == null).ToList();
            _context.Purchases.RemoveRange(purchases);
            _context.SaveChanges();
        }

        public bool DeleteRange(List<Purchase> purchases)
        {
            _context.Purchases.RemoveRange(purchases);
            return _context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Purchase GetPurchase(int id)
        {
            return _context.Purchases.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Purchase> GetPurchases(IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if(userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);
            return _context.Purchases.Where(x => x.PurchasedBy.Id == id);
        }

        public Purchase RedeemVoucher(string voucherCode, IEnumerable<Claim> claims) {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if(userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if(user == null) throw new ApiException($"The user could not be found");

            var voucher = _context.Vouchers.FirstOrDefault(x => x.Code.Equals(voucherCode));
            if(voucher == null) throw new ApiException($"Voucher '{voucherCode}' does not exist!");
            if(voucher.User != null) throw new ApiException($"Voucher has already been redeemed!");

            var purchase = new Purchase
            {
                DateCreated = DateTime.UtcNow,
                NumberOfTickets = voucher.Product.NumberOfTickets,
                OrderId = voucherCode,
                Price = 0,
                ProductId = voucher.Product.Id,
                ProductName = voucher.Product.Name,
                PurchasedBy = user
            };

            user.Purchases.Add(purchase);
            
            DeliverProductToUser(purchase, user, $"VOUCHER: {voucher.Id}");

            voucher.DateUsed = DateTime.UtcNow;
            voucher.User = user;

            _context.Vouchers.Attach(voucher);
            _context.Entry(voucher).State = EntityState.Modified;
            _context.SaveChanges();

            return purchase;
        }

        public void DeliverProduct(CompletePurchaseDTO completeDto, IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if(userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if(user == null) throw new ApiException($"The user could not be found");

            var purchase = user.Purchases.Where(x => x.OrderId == completeDto.OrderId).FirstOrDefault();
            if (purchase == null) throw new ApiException($"Purchase could not be found");

            DeliverProductToUser(purchase, user, completeDto.TransactionId);
        }

        public void DeliverProductToUser(Purchase purchase, User user, string transactionId)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == purchase.ProductId);
            if (product == null) throw new ApiException($"The product with id {purchase.ProductId} could not be found!");
            for (var i = 0; i < purchase.NumberOfTickets; i++)
            {
                var ticket = new Ticket() { ProductId = product.Id, Purchase = purchase };
                user.Tickets.Add(ticket);
            }

            purchase.TransactionId = transactionId;
            purchase.Completed = true;
            
            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public string InitiatePurchase(int productId, IEnumerable<Claim> claims) {
            var product = _context.Products.FirstOrDefault(x => x.Id == productId);
            if (product == null) throw new ApiException($"Product with id {productId} could not be found!", 400);

            var orderId = "";

            while(orderId == "") {
                var guid = Guid.NewGuid();
                if(!_context.Purchases.Any(x => x.OrderId.Equals(guid))) orderId = guid.ToString();
            }

            var purchase = new Purchase()
            {
                OrderId = orderId,
                Price = product.Price,
                ProductName = product.Name,
                ProductId = productId,
                NumberOfTickets = product.NumberOfTickets
            };

            var userId = claims.FirstOrDefault(x => x.Type == Constants.UserId);
            if(userId == null) throw new ApiException($"The token is invalid!", 401);
            var id = int.Parse(userId.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if(user == null) throw new ApiException($"The user could not be found");

            user.Purchases.Add(purchase);

            _context.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            return orderId;
        }
    }
}
