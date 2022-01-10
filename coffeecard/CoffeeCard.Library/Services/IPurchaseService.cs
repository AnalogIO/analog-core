using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IPurchaseService : IDisposable
    {
        bool Delete(int id);
        Purchase Read(string orderId);
        IEnumerable<Purchase> Read(DateTime from, DateTime to);
        IEnumerable<Purchase> Read(DateTime from);
        Purchase GetPurchase(int id);
        IEnumerable<Purchase> GetPurchases(IEnumerable<Claim> claims);
        Purchase RedeemVoucher(string voucherCode, IEnumerable<Claim> claims);
        string InitiatePurchase(int productId, IEnumerable<Claim> claims);
        Task<Purchase> CompletePurchase(CompletePurchaseDto dto, IEnumerable<Claim> claims);
        //Task CheckIncompletePurchases(User user); //TODO Reimplement
        Purchase DeliverProduct(CompletePurchaseDto completeDto, IEnumerable<Claim> claims);
        Purchase DeliverProductToUser(Purchase purchase, User user, string transactionId);
        int Update(Purchase purchase);
        void Update();
        bool DeleteRange(List<Purchase> purchases);
        Purchase IssueProduct(IssueProductDto issueProduct);
    }
}