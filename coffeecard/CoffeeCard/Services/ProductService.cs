using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Services
{
    public class ProductService : IProductService
    {
        private readonly CoffeeCardContext _context;

        public ProductService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetPublicProducts()
        {
            return await GetProducts(false);
        }

        public async Task<IEnumerable<Product>> GetProductsForUserAsync(User user)
        {
            return await GetProducts(user.IsBarista);
        }

        private async Task<IEnumerable<Product>> GetProducts(bool includeBaristaProducts)
        {
            var visibleProducts = _context.Products.Where(p => p.Visible);

            if (!includeBaristaProducts)
            {
                return await visibleProducts.Where(p => p.BaristasOnly == false).ToListAsync();
            }

            return await visibleProducts.ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}