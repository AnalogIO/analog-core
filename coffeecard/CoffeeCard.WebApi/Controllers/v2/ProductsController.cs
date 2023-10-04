using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Leaderboard;
using CoffeeCard.Models.DataTransferObjects.v2.Product;
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
        private readonly ClaimsUtilities _claimsUtilities;
        private readonly IMapperService _mapperService;
        private readonly IProductService _productService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(IProductService productService, IMapperService mapper,
            ClaimsUtilities claimsUtilities)
        {
            _productService = productService;
            _mapperService = mapper;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="initiateProductRequest">The request containing the details of the product to be added and allowed user groups.</param>
        /// <returns> The newly added product wrapped in a InitiateProductResponse object.</returns>
        /// <response code="200">The request was successful, and the product was added.</response>
        [HttpPost("add")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(InitiateProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProduct(InitiateProductRequestWithUserGroups initiateProductRequest)
        {
            return Ok(await _productService.AddProduct(initiateProductRequest.Product, initiateProductRequest.AllowedUserGroups));
        }
        
        
        /// <summary>
        /// Updates a product with the specified changes.
        /// </summary>
        /// <param name="product">The request containing the changes to be applied to the product.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        /// <response code="200">The request was successful, and the product was updated.</response>
        [HttpGet("update")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(InitiateProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProduct(InitiateProductRequest product)
        {
            return Ok(await _productService.UpdateProduct(product));
        }
        
        
        /// <summary>
        /// Deactivates a product by setting its visibility to false.
        /// </summary>
        /// <param name="productId">The ID of the product to be deactivated.</param>
        /// <returns>A response indicating the result of the deactivation operation.</returns>
        /// <response code="200">The request was successful, and the product was deactivated.</response>
        /// <response code="404">The product with the specified ID was not found.</response>
        [HttpPost("deactivate/{productId}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateProduct(int productId)
        {
            var result = await _productService.DeactivateProduct(productId);

            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }            
}