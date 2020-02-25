using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Models;

namespace CoffeeCard.Services
{
    public class ProductService : IProductService
    {
        private readonly CoffeeCardContext _context;

        public ProductService(CoffeeCardContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.AsEnumerable();
        }
    }
}