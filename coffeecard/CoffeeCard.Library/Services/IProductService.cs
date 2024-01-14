using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services
{
    public interface IProductService : IDisposable
    {
        Task<IEnumerable<Product>> GetPublicProducts();
        Task<IEnumerable<Product>> GetProductsForUserAsync(User user);
    }
}
