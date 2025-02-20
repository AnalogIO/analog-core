using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<IEnumerable<IssueVoucherResponse>> CreateVouchers(IssueVoucherRequest request, User user)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                throw new EntityNotFoundException("No product was found by Id " + request.ProductId);
            }

            var newCodes = new HashSet<string>();
            var existingVouchers = _context.Vouchers.Where(v => v.Code.StartsWith(request.VoucherPrefix)).Select(v => v.Code).ToHashSet();
            while (newCodes.Count < request.Amount)
            {
                var code = GenerateUniqueVoucherCode(8, request.VoucherPrefix, existingVouchers); // 8 character length gives 36^8 combos
                newCodes.Add(code);
            }

            var vouchers = newCodes
                .Select(code => new Voucher
                {
                    IssuerId = user.Id,
                    Code = code,
                    DateCreated = DateTime.UtcNow,
                    Product = product,
                    Description = request.Description,
                    Requester = request.Requester
                }).ToList();

            await _context.Vouchers.AddRangeAsync(vouchers);
            await _context.SaveChangesAsync();

            var responses = vouchers.
                Select(v =>
                    new IssueVoucherResponse
                    {
                        VoucherCode = v.Code,
                        IssuedAt = v.DateCreated,
                        ProductId = v.Product.Id,
                        ProductName = v.Product.Name
                    });
            return responses;
        }

        public async Task<VoucherListResponse> GetVouchers(int pageNum, int pageLength)
        {
            int skip = pageNum * pageLength;

            IQueryable<Voucher> query = _context.Vouchers.Include(v => v.User);

            var totalVouchers = await query.CountAsync();

            if (totalVouchers < skip)
            {
                throw new BadRequestException($"The value of {nameof(pageNum)} is outside of the range of total users");
            }

            var vouchersByPage = await query
                .Include(v => v.User)
                .Include(v => v.Product)
                .OrderByDescending(v => v.Id) // Newest first
                .Skip(skip).Take(pageLength)
                .Select(v => new VoucherResponse
                {
                    Requester = v.Requester,
                    Description = v.Description,
                    Code = v.Code,
                    Id = v.Id,
                    Status = v.DateUsed == null ? VoucherStatus.UNUSED : VoucherStatus.USED,
                    ProductName = v.Product.Name,
                    UsedBy = v.User.Name
                })
                .ToListAsync();

            return new VoucherListResponse
            {
                TotalVouchers = totalVouchers,
                Vouchers = vouchersByPage
            };
        }

        /// <summary>
        /// Generates a unique voucher code, that is not currently in the database
        /// </summary>
        /// <param name="codeLength">The length of the generated code</param>
        /// <param name="voucherPrefix">The user defined prefix of the generated code</param>
        /// <param name="existingCodes">A set of existing voucher codes</param>
        /// <returns>A string representing a voucher code</returns>
        private string GenerateUniqueVoucherCode(int codeLength, string voucherPrefix, HashSet<string> existingCodes)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder code = new StringBuilder();

            while (String.IsNullOrEmpty(code.ToString()) || existingCodes.Contains(code.ToString()))
            {
                code.Append($"{voucherPrefix}-"); // Ensure code starts with prefix

                for (var i = 0; i < codeLength; i++)
                {
                    code.Append(chars[_random.Next(chars.Length)]);
                }
            }
            return code.ToString();
        }
    }
}