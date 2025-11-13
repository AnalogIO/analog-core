using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IPurchaseService : IDisposable
    {
        Task<Purchase> RedeemVoucher(string voucherCode, IEnumerable<Claim> claims);
    }
}
