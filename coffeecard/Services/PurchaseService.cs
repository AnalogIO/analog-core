using coffeecard.Helpers;
using Coffeecard.Models;
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
            var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null) throw new ApiException($"The token is invalid!", 401);
            var email = emailClaim.Value;
            return _context.Purchases.Where(x => x.PurchasedBy.Email == email);
        }
    }
}
