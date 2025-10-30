using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;

namespace CoffeeCard.Library.Services.v2
{
    public interface IProductService : IDisposable
    {
        Task<IEnumerable<ProductResponse>> GetProductsForUserAsync(User user);
        Task<ProductResponse> GetProductAsync(int productId);
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse> AddProduct(AddProductRequest product);
        Task<ProductResponse> UpdateProduct(int productId, UpdateProductRequest product);
    }
}
