using Coffeecard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
    }
}
