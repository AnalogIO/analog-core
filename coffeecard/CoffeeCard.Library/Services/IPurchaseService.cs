using CoffeeCard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services
{
    public interface IPurchaseService : IDisposable
    {
        Task<Purchase> RedeemVoucher(string voucherCode, IEnumerable<Claim> claims);
    }
}