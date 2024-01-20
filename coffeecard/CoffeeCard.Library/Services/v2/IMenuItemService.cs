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
        Task<MenuItemResponse> AddMenuItem(AddMenuItemRequest newMenuItem);
        Task<ChangedMenuItemResponse> UpdateMenuItem(UpdateMenuItemRequest changedMenuItem);
    }
}