using System.Collections.Generic;
using System.Data;
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
            return await _context.Products
                .Where(p => p.ProductUserGroup.Any(pug => pug.UserGroup == userGroup))
                .Where(p => p.Visible).OrderBy(p => p.Id)
                .Include(p => p.ProductUserGroup)
                .ToListAsync();
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

        private async Task<bool> CheckProductUniquenessAsync(string name, int price)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => (p.Name == name && p.Price == price));

            return product == null;
        }

        public async Task<ChangedProductResponse> AddProduct(AddProductRequest newProduct)
        {
            var unique = await CheckProductUniquenessAsync(newProduct.Name, newProduct.Price);
            if (!unique)
            {
                throw new ConflictException($"Product already exists with name {newProduct.Name} and price of {newProduct.Price}");
            }

            var product = new Product()
            {
                Price = newProduct.Price,
                Description = newProduct.Description,
                Name = newProduct.Name,
                NumberOfTickets = newProduct.NumberOfTickets,
                ExperienceWorth = 0,
                Visible = newProduct.Visible
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productUserGroups = newProduct.AllowedUserGroups.Select(userGroup => new ProductUserGroup
            {
                ProductId = product.Id,
                UserGroup = userGroup
            }).ToList();

            _context.ProductUserGroups.AddRange(productUserGroups);


            await _context.SaveChangesAsync();

            var result = new ChangedProductResponse
            {
                Price = product.Price,
                Description = product.Description,
                Name = product.Name,
                NumberOfTickets = product.NumberOfTickets,
                Visible = product.Visible,
                AllowedUserGroups = newProduct.AllowedUserGroups
            };

            return result;
        }

        public async Task<ChangedProductResponse> UpdateProduct(UpdateProductRequest changedProduct)
        {
            var newProductUserGroups = changedProduct.AllowedUserGroups.Select(userGroup => new ProductUserGroup
            {
                ProductId = changedProduct.Id,
                UserGroup = userGroup
            }).ToList();

            var product = await GetProductAsync(changedProduct.Id);
            product.Price = changedProduct.Price;
            product.Description = changedProduct.Description;
            product.NumberOfTickets = changedProduct.NumberOfTickets;
            product.Name = changedProduct.Name;
            product.Visible = changedProduct.Visible;
            product.ProductUserGroup = newProductUserGroups;

            await _context.SaveChangesAsync();

            var result = new ChangedProductResponse
            {
                Price = product.Price,
                Description = product.Description,
                Name = product.Name,
                NumberOfTickets = product.NumberOfTickets,
                Visible = product.Visible,
                AllowedUserGroups = changedProduct.AllowedUserGroups
            };

            return result;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}