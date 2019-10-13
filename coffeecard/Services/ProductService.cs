using Coffeecard.Models;
using System.Collections.Generic;
using System.Linq;

namespace coffeecard.Services
{
    public class ProductService : IProductService
    {
        private readonly CoffeecardContext _context;
        public ProductService(CoffeecardContext context)
        {
            _context = context;
        }
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.AsEnumerable();
        }
    }
}
