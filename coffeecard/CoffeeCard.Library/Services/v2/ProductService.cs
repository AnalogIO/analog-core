using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class ProductService : IProductService
    {
        private readonly CoffeeCardContext _context;

        public ProductService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetPublicProductsAsync()
        {
            return await GetProductsAsync(UserGroup.Customer);
        }

        public async Task<IEnumerable<Product>> GetProductsForUserAsync(User user)
        {
            return await GetProductsAsync(user.UserGroup);
        }

        private async Task<IEnumerable<Product>> GetProductsAsync(UserGroup userGroup)
        {
            return await _context.Products
                .Where(p => p.ProductUserGroup.Any(pug => pug.UserGroup == userGroup))
                .Where(p => p.Visible).OrderBy(p => p.Id)
                .Include(p => p.ProductUserGroup)
                .ToListAsync();
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductUserGroup)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                Log.Error("No product was found by Product Id: {Id}", productId);
                throw new EntityNotFoundException($"No product was found by Product Id: {productId}");
            }

            return product;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}