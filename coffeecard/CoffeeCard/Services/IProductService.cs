using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models;

namespace CoffeeCard.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetPublicProducts();
        Task<IEnumerable<Product>> GetProductsForUserAsync(User user);
    }
}