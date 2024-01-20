using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CoffeeCard.Library.Services.v2
{
    public sealed class MenuItemService : IMenuItemService
    {
        private readonly CoffeeCardContext _context;

        public MenuItemService(CoffeeCardContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemResponse>> GetAllMenuItemsAsync()
        {
            return await _context.MenuItems
                .OrderBy(p => p.Id)
                .Select(p => new MenuItemResponse
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }


        private async Task<bool> CheckMenuItemUniquenessAsync(string name, int id = 0)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(p => p.Name == name && p.Id != id);

            return menuItem == null;
        }

        public async Task<MenuItemResponse> AddMenuItem(AddMenuItemRequest newMenuItem)
        {
            var unique = await CheckMenuItemUniquenessAsync(newMenuItem.Name);
            if (!unique)
            {
                throw new ConflictException($"Menu item already exists with name {newMenuItem.Name}");
            }

            var menuItem = new MenuItem()
            {
                Name = newMenuItem.Name
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var result = new MenuItemResponse
            {
                Id = menuItem.Id,
                Name = menuItem.Name
            };

            return result;
        }

        public async Task<MenuItemResponse> UpdateMenuItem(int id, UpdateMenuItemRequest changedMenuItem)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(p => p.Id == id);

            if (menuItem == null)
            {
                Log.Error("No menu item was found by Menu Item Id: {Id}", id);
                throw new EntityNotFoundException($"No menu item was found by Menu Item Id: {id}");
            }

            var unique = await CheckMenuItemUniquenessAsync(changedMenuItem.Name, id);
            if (!unique)
            {
                throw new ConflictException($"Menu item already exists with name {changedMenuItem.Name}");
            }

            menuItem.Name = changedMenuItem.Name;

            await _context.SaveChangesAsync();

            var result = new MenuItemResponse
            {
                Id = id,
                Name = menuItem.Name
            };

            return result;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
