using Coffeecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface IPurchaseService
    {
        bool Delete(int id);
        Purchase Read(string orderId);
        Purchase GetPurchase(int id);
        List<Purchase> GetPurchases(int userId);
        List<Purchase> Read(DateTime from, DateTime to);
        List<Purchase> Read(DateTime from);
        int Update(Purchase purchase);
        void Update();
        bool DeleteRange(List<Purchase> purchases);
        void Dispose();

    }
}
