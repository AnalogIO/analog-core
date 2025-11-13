using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.Library.Services
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
            return await GetProducts(UserGroup.Customer);
        }

        public async Task<IEnumerable<Product>> GetProductsForUserAsync(User user)
        {
            return await GetProducts(user.UserGroup);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task<IEnumerable<Product>> GetProducts(UserGroup userGroup)
        {
            return await (
                from p in from pug in _context.ProductUserGroups
                where pug.UserGroup == userGroup
                select pug.Product
                where p.Visible
                orderby p.Id
                select p
            ).ToListAsync();
        }
    }
}
