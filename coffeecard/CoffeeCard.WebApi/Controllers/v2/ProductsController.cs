using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
using CoffeeCard.Models.DataTransferObjects.v2.Products;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IProductService = CoffeeCard.Library.Services.v2.IProductService;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for creating, changing, and deactivating a product
    /// </summary>
    [ApiController]
    [Authorize]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ClaimsUtilities _claimsUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(IProductService productService, ClaimsUtilities claimsUtilities)
        {
            _productService = productService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="addProductRequest">The request containing the details of the product to be added and allowed user groups.</param>
        /// <returns> The newly added product.</returns>
        /// <response code="200">The request was successful, and the product was added.</response>
        [HttpPost("")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProduct(AddProductRequest addProductRequest)
        {
            return Ok(MapProductToDto(await _productService.AddProduct(addProductRequest)));
        }

        /// <summary>
        /// Updates a product with the specified changes.
        /// </summary>
        /// <param name="product">The request containing the changes to be applied to the product.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        /// <response code="200">The request was successful, and the product was updated.</response>
        [HttpPut("")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProduct(UpdateProductRequest product)
        {
            return Ok(MapProductToDto(await _productService.UpdateProduct(product)));
        }

        /// <summary>
        /// Returns a list of available products based on a account's user group.
        /// </summary>
        /// <returns>List of available products</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            var products = await _productService.GetProductsForUserAsync(user);
            return Ok(products.Select(MapProductToDto).ToList());
        }

        /// <summary>
        /// Returns a product with the specified id
        /// </summary>
        /// <param name="id">The id of the product to be returned</param>
        /// <returns>The product with the specified id</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">The product with the specified id could not be found</response>
        /// <response code="403">The user is not allowed to access the product</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute(Name = "id")] int id)
        {
            await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            var product = await _productService.GetProductAsync(id);
            return Ok(MapProductToDto(product));
        }

        private static ProductResponse MapProductToDto(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                NumberOfTickets = product.NumberOfTickets,
                Price = product.Price,
                IsPerk = product.IsPerk(),
                Visible = product.Visible,
                AllowedUserGroups = product.ProductUserGroup.Select(e => e.UserGroup),
                EligibleMenuItems = product.EligibleMenuItems.Select(
                    item => new MenuItemResponse { Id = item.Id, Name = item.Name }
                )
            };
        }

        /// <summary>
        /// Returns a list of all products
        /// </summary>
        /// <returns>List of all products</returns>
        /// <response code="200">Successful request</response>
        [HttpGet("all")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts()
        {
            IEnumerable<Product> products = await _productService.GetAllProductsAsync();
            return Ok(products.Select(MapProductToDto).ToList());
        }
    }
}
