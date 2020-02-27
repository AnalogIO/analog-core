﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay;
using CoffeeCard.WebApi.Models.DataTransferObjects.Purchase;

namespace CoffeeCard.WebApi.Services
{
    public interface IPurchaseService
    {
        bool Delete(int id);
        Purchase Read(string orderId);
        Purchase GetPurchase(int id);
        IEnumerable<Purchase> GetPurchases(IEnumerable<Claim> claims);
        Purchase RedeemVoucher(string voucherCode, IEnumerable<Claim> claims);
        string InitiatePurchase(int productId, IEnumerable<Claim> claims);

        Task<Purchase> CompletePurchase(CompletePurchaseDTO dto, IEnumerable<Claim> claims);

        //Task CheckIncompletePurchases(User user); //TODO Reimplement
        Purchase DeliverProduct(CompletePurchaseDTO completeDto, IEnumerable<Claim> claims);
        Purchase DeliverProductToUser(Purchase purchase, User user, string transactionId);
        IEnumerable<Purchase> Read(DateTime from, DateTime to);
        IEnumerable<Purchase> Read(DateTime from);
        int Update(Purchase purchase);
        void Update();
        bool DeleteRange(List<Purchase> purchases);
        void Dispose();
        Purchase IssueProduct(IssueProductDTO issueProduct);
    }
}