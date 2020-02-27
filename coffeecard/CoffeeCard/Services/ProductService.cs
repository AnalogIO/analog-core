using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCard.WebApi.Services
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

        private async Task<IEnumerable<Product>> GetProducts(UserGroup userGroup)
        {
            return await
                (
                    from p in (
                        from pug in _context.ProductUserGroups
                        where pug.UserGroup == userGroup
                        select pug.Product)
                    where p.Visible
                    orderby p.Id
                    select p
                ).ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}