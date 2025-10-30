using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Models.DataTransferObjects.v2.MenuItems;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IMenuItemService = CoffeeCard.Library.Services.v2.IMenuItemService;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for creating, changing, and deactivating a product
    /// </summary>
    [ApiController]
    [Authorize]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/menuitems")]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemsController"/> class.
        /// </summary>
        public MenuItemsController(IMenuItemService menuItemsService)
        {
            _menuItemService = menuItemsService;
        }

        /// <summary>
        /// Returns a list of all menu items
        /// </summary>
        /// <returns>List of all menu items</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">User not allowed to access menu items</response>
        [HttpGet]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(IEnumerable<MenuItemResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MenuItemResponse>>> GetAllMenuItems()
        {
            return Ok(await _menuItemService.GetAllMenuItemsAsync());
        }

        /// <summary>
        /// Adds a menu item
        /// </summary>
        /// <param name="menuItem">Menu item to add</param>
        /// <returns>Menu item</returns>
        /// <response code="201">Menu item successfully added</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<MenuItemResponse>> AddMenuItem(AddMenuItemRequest menuItem)
        {
            return CreatedAtAction(
                nameof(GetAllMenuItems),
                await _menuItemService.AddMenuItemAsync(menuItem)
            );
        }

        /// <summary>
        /// Updates a menu item
        /// </summary>
        /// <param name="id">Menu item id to update</param>
        /// <param name="menuItem">Menu item to update</param>
        /// <returns>Menu item</returns>
        /// <response code="200">Menu item successfully updated</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">Menu item not found</response>
        /// <response code="409">Menu item is in use and cannot be disabled</response>
        /// <response code="409">Menu item with the same name already exists</response>
        [HttpPut("{id}")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MenuItemResponse>> UpdateMenuItem(
            [FromRoute] int id,
            UpdateMenuItemRequest menuItem
        )
        {
            return Ok(await _menuItemService.UpdateMenuItemAsync(id, menuItem));
        }
    }
}
