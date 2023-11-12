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
        /// <returns> The newly added product wrapped in a InitiateProductResponse object.</returns>
        /// <response code="201">Product created</response>
        /// <response code="409">Product name already exists</response>
        [HttpPost]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(DetailedProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DetailedProductResponse>> AddProduct([FromBody] AddProductRequest addProductRequest)
        {
            var product = await _productService.AddProduct(addProductRequest);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id, version = 2 }, product);
        }

        /// <summary>
        /// Updates a product with the specified changes.
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <param name="product">The request containing the changes to be applied to the product.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        /// <response code="200">Product updated</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id:int}")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(DetailedProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailedProductResponse>> UpdateProduct(int id, [FromBody] UpdateProductRequest product)
        {
            return Ok(await _productService.UpdateProduct(id, product));
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        /// <param name="id">Product Id</param>
        /// <returns>Detailed Product</returns>
        /// <response code="200">Detailed Product</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DetailedProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DetailedProductResponse>> GetProductById(int id)
        {
            return Ok(await _productService.GetProductAsync(id));
        }

        /// <summary>
        /// Returns a list of available products based on a account's user group
        /// </summary>
        /// <returns>List of available products</returns>
        /// <response code="200">Successful request</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<SimpleProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SimpleProductResponse>>> GetProducts()
        {
            IEnumerable<Product> products;
            try
            {
                // Try find user from potential login token
                var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
                products = await _productService.GetProductsForUserAsync(user);
            }
            catch (ApiException)
            {
                // No token found, retrieve customer products
                products = await _productService.GetPublicProductsAsync();
            }


            return Ok(products.Select(MapProductToDto).ToList());
        }

        private static SimpleProductResponse MapProductToDto(Product product)
        {
            return new SimpleProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                NumberOfTickets = product.NumberOfTickets,
                Price = product.Price,
                IsPerk = product.IsPerk()
            };
        }
    }
}
