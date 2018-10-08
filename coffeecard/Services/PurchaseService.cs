using Coffeecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Purchase Read(int id)
        {
            return _context.Purchases.Find(id);
        }

        public List<Purchase> Read()
        {
            return _context.Purchases.ToList();
        }

        public List<Purchase> Read(DateTime from, DateTime to)
        {
            return _context.Purchases.Where(x => x.DateCreated >= from && x.DateCreated <= to).ToList();
        }

        public List<Purchase> Read(DateTime from)
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
    }
}
