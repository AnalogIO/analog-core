using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class ProductService : IProductService
    {
        private readonly CoffeeCardContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(CoffeeCardContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsForUserAsync(User user)
        {
            return await GetProductsAsync(user.UserGroup);
        }

        private async Task<IEnumerable<ProductResponse>> GetProductsAsync(UserGroup userGroup)
        {
            return await _context
                .Products.Where(p => p.ProductUserGroup.Any(pug => pug.UserGroup == userGroup))
                .Where(p => p.Visible)
                .OrderBy(p => p.Id)
                .Include(p => p.ProductUserGroup)
                .Include(p => p.EligibleMenuItems)
                .Select(p => p.ToProductResponse())
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            return await _context
                .Products.OrderBy(p => p.Id)
                .Include(p => p.ProductUserGroup)
                .Include(p => p.EligibleMenuItems)
                .Select(p => p.ToProductResponse())
                .ToListAsync();
        }

        public async Task<ProductResponse> GetProductAsync(int productId)
        {
            var product = await _context
                .Products.Include(p => p.ProductUserGroup)
                .Include(p => p.EligibleMenuItems)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                _logger.LogError("No product was found by Product Id: {Id}", productId);
                throw new EntityNotFoundException(
                    $"No product was found by Product Id: {productId}"
                );
            }

            return product.ToProductResponse();
        }

        private async Task<bool> CheckProductUniquenessAsync(string name, int price)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p =>
                (p.Name == name && p.Price == price)
            );

            return product == null;
        }

        public async Task<ProductResponse> AddProduct(AddProductRequest newProduct)
        {
            var unique = await CheckProductUniquenessAsync(newProduct.Name, newProduct.Price);
            if (!unique)
            {
                throw new ConflictException(
                    $"Product already exists with name {newProduct.Name} and price of {newProduct.Price}"
                );
            }

            var product = new Product()
            {
                Price = newProduct.Price,
                Description = newProduct.Description,
                Name = newProduct.Name,
                NumberOfTickets = newProduct.NumberOfTickets,
                ExperienceWorth = 0,
                Visible = newProduct.Visible,
                ProductUserGroup = newProduct
                    .AllowedUserGroups.Select(userGroup => new ProductUserGroup
                    {
                        UserGroup = userGroup,
                    })
                    .ToList(),
                EligibleMenuItems = _context
                    .MenuItems.Where(mi => newProduct.MenuItemIds.Contains(mi.Id))
                    .ToList(),
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product.ToProductResponse();
        }

        public async Task<ProductResponse> UpdateProduct(
            int productId,
            UpdateProductRequest changedProduct
        )
        {
            var product = await _context
                .Products.Include(p => p.ProductUserGroup)
                .Include(p => p.EligibleMenuItems)
                .FirstOrDefaultAsync(p => p.Id == productId);

            product.Price = changedProduct.Price;
            product.Description = changedProduct.Description;
            product.NumberOfTickets = changedProduct.NumberOfTickets;
            product.Name = changedProduct.Name;
            product.Visible = changedProduct.Visible;
            product.ProductUserGroup = changedProduct
                .AllowedUserGroups.Select(userGroup => new ProductUserGroup
                {
                    ProductId = product.Id,
                    UserGroup = userGroup,
                })
                .ToList();
            product.EligibleMenuItems = _context
                .MenuItems.Where(mi => changedProduct.MenuItemIds.Contains(mi.Id))
                .ToList();

            await _context.SaveChangesAsync();

            return product.ToProductResponse();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
