using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;

namespace CoffeeCard.Library.Services.v2
{
    public interface IMenuItemService : IDisposable
    {
        Task<IEnumerable<MenuItemResponse>> GetAllMenuItemsAsync();
        Task<MenuItemResponse> AddMenuItemAsync(AddMenuItemRequest newMenuItem);
        Task<MenuItemResponse> UpdateMenuItemAsync(int menuItemid, UpdateMenuItemRequest changedMenuItem);
        Task<MenuItemResponse> DisableMenuItemAsync(int id);
    }
}