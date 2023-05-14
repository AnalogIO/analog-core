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
        Purchase RedeemVoucher(string voucherCode, IEnumerable<Claim> claims);
    }
}