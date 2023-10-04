using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class ProductService : IProductService
    {
        private readonly CoffeeCardContext _context;

        public ProductService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetPublicProductsAsync()
        {
            return await GetProductsAsync(UserGroup.Customer);
        }

        public async Task<IEnumerable<Product>> GetProductsForUserAsync(User user)
        {
            return await GetProductsAsync(user.UserGroup);
        }

        private async Task<IEnumerable<Product>> GetProductsAsync(UserGroup userGroup)
        {
            return await
            (
                from p in from pug in _context.ProductUserGroups
                          where pug.UserGroup == userGroup
                          select pug.Product
                where p.Visible
                orderby p.Id
                select p
            ).ToListAsync();
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductUserGroup)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                Log.Error("No product was found by Product Id: {Id}", productId);
                throw new EntityNotFoundException($"No product was found by Product Id: {productId}");
            }

            return product;
        }

        public Task<InitiateProductResponse> AddProduct(InitiateProductRequest newProduct, IEnumerable<UserGroup> allowedUserGroups)
        {
            var product = new Product()
            {
                Price = newProduct.Price,
                Description = newProduct.Description,
                Name = newProduct.Name,
                NumberOfTickets = newProduct.NumberOfTickets,
                Visible = newProduct.Visible
            };
            
            var productUserGroups = allowedUserGroups.Select(userGroup => new ProductUserGroup
            {
                ProductId = product.Id,
                UserGroup = userGroup
            }).ToList();

            _context.ProductUserGroups.AddRange(productUserGroups);
            
            _context.Products.Add(product);
            
            _context.SaveChanges();


            var result = new InitiateProductResponse
            {
                Price = product.Price,
                Description = product.Description,
                Name = product.Name,
                NumberOfTickets = product.NumberOfTickets,
                Visible = product.Visible
            };

            return Task.FromResult(result);
        }
        
        private Product GetProduct(int productId)
        {
            return _context.Products.Where(p => p.Id == productId).FirstOrDefault();
        }
        
        public Task<InitiateProductResponse> UpdateProduct(InitiateProductRequest changedProduct)
        {
            var product = GetProduct(changedProduct.Id);

            if (changedProduct.Id != default(int))
            {
                Log.Information($"Changing Id of product from {product.Id} to {changedProduct.Id}");
                product.Id = changedProduct.Id;
            }

            if (changedProduct.NumberOfTickets != default(int))
            {
                Log.Information($"Changing NumberOfTickets of product from {product.NumberOfTickets} to {changedProduct.NumberOfTickets}");
                product.NumberOfTickets = changedProduct.NumberOfTickets;
            }

            if (!string.IsNullOrEmpty(changedProduct.Name))
            {
                Log.Information($"Changing Name of product from {product.Name} to {changedProduct.Name}");
                product.Name = changedProduct.Name;
            }

            if (changedProduct.ExperienceWorth != default(int))
            {
                Log.Information($"Changing ExperienceWorth of product from {product.ExperienceWorth} to {changedProduct.ExperienceWorth}");
                product.ExperienceWorth = changedProduct.ExperienceWorth;
            }

            if (changedProduct.Visible != default(bool))
            {
                Log.Information($"Changing Visible of product from {product.Visible} to {changedProduct.Visible}");
                product.Visible = changedProduct.Visible;
            }

            _context.SaveChanges();
            
            var result = new InitiateProductResponse
            {
                Price = product.Price,
                Description = product.Description,
                Name = product.Name,
                NumberOfTickets = product.NumberOfTickets,
                Visible = product.Visible
            };
            
            return Task.FromResult(result);
        }

        public async Task<bool> DeactivateProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return false;
            }
    
            product.Visible = false;
    
            await _context.SaveChangesAsync();

            return true;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}