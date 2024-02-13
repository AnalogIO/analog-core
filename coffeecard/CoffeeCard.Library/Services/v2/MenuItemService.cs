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

        public async Task<MenuItemResponse> AddMenuItemAsync(AddMenuItemRequest newMenuItem)
        {
            var nameExists = await CheckMenuItemNameExists(newMenuItem.Name);
            if (nameExists)
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

        private async Task<bool> CheckMenuItemNameExists(string name)
        {
            return await _context.MenuItems
                .AnyAsync(p => p.Name == name);
        }

        public async Task<MenuItemResponse> UpdateMenuItemAsync(int id, UpdateMenuItemRequest changedMenuItem)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(p => p.Id == id);

            if (menuItem == null)
            {
                Log.Warning("No menu item was found by Menu Item Id: {Id}", id);
                throw new EntityNotFoundException($"No menu item was found by Menu Item Id {id}");
            }

            var nameExists = await CheckMenuItemNameExists(changedMenuItem.Name);
            if (nameExists)
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

        public async Task<MenuItemResponse> DisableMenuItemAsync(int id)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(p => p.Id == id);

            if (menuItem == null)
            {
                Log.Warning("No menu item was found by Menu Item Id: {Id}", id);
                throw new EntityNotFoundException($"No menu item was found by Menu Item Id {id}");
            }

            if (await _context.Products.AnyAsync(p => p.EligibleMenuItems.Any(menuItem => menuItem.Id == id)))
            {
                throw new ConflictException("Menu item is in use and cannot be disabled");
            }

            menuItem.Active = false;
            await _context.SaveChangesAsync();
            var result = new MenuItemResponse
            {
                Id = id,
                Name = menuItem.Name,
                Active = false
            };
            return result;

        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
