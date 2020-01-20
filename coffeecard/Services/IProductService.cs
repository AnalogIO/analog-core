using System.Collections.Generic;
using CoffeeCard.Models;

namespace CoffeeCard.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
    }
}
