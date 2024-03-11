using CoffeeCard.Models.DataTransferObjects.v2.MenuItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeCard.Library.Services.v2
{
    public interface IMenuItemService : IDisposable
    {
        Task<IEnumerable<MenuItemResponse>> GetAllMenuItemsAsync();
        Task<MenuItemResponse> AddMenuItemAsync(AddMenuItemRequest newMenuItem);
        Task<MenuItemResponse> UpdateMenuItemAsync(int menuItemid, UpdateMenuItemRequest changedMenuItem);
    }
}