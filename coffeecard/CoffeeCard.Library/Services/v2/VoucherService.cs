using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Voucher;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services.v2
{
    public class VoucherService : IVoucherService
    {
        private readonly CoffeeCardContext _context;
        private readonly Random _random = new Random();

        public VoucherService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IssueVoucherResponse>> CreateVouchers(IssueVoucherRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                throw new ApiException("No product was found with given Id", StatusCodes.Status404NotFound);
            }

            var codes = new HashSet<string>();
            while (codes.Count < request.Amount)
            {
                codes.Add(await GenerateUniqueVoucherCode(6)); // 6 character length gives 36^6 combos
            }
            
            var vouchers = codes
                .Select( code => new Voucher
                {
                    Code = code,
                    DateCreated = DateTime.Now,
                    Product = product,
                }).ToList();

            await _context.Vouchers.AddRangeAsync(vouchers);
            await _context.SaveChangesAsync();

            var responses = vouchers.
                Select(v => new IssueVoucherResponse { VoucherCode = v.Code });
            return responses;
        }

        /// <summary>
        /// Generates a unique voucher code, that is not currently in the database
        /// </summary>
        /// <param name="codeLength"></param>
        /// <returns>A string representing a voucher code</returns>
        private async Task<string> GenerateUniqueVoucherCode(int codeLength)
        {
            while (true)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                var code = new string(Enumerable.Repeat(chars, codeLength)
                    .Select(s => s[_random.Next(s.Length)])
                    .ToArray());

                var exits = await _context.Vouchers.AnyAsync(v => v.Code == code);

                if (!exits) return code;
            }
        }
    }
}