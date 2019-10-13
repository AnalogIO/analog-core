using Coffeecard.Models;
using System.Collections.Generic;

namespace coffeecard.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
    }
}
