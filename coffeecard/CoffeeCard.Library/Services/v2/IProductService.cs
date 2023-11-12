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
        Task<IEnumerable<Product>> GetPublicProductsAsync();
        Task<IEnumerable<Product>> GetProductsForUserAsync(User user);
        Task<Product> GetProductAsync(int productId);
        Task<DetailedProductResponse> AddProduct(AddProductRequest product);
        Task<DetailedProductResponse> UpdateProduct(int productId, UpdateProductRequest product);
    }
}