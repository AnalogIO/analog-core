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
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="addProductRequest">The request containing the details of the product to be added and allowed user groups.</param>
        /// <returns> The newly added product wrapped in a InitiateProductResponse object.</returns>
        /// <response code="200">The request was successful, and the product was added.</response>
        [HttpPost("")]
        //[AuthorizeRoles(UserGroup.Board)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProduct(AddProductRequest addProductRequest)
        {
            return Ok(await _productService.AddProduct(addProductRequest));
        }


        /// <summary>
        /// Updates a product with the specified changes.
        /// </summary>
        /// <param name="product">The request containing the changes to be applied to the product.</param>
        /// <returns>A response indicating the result of the update operation.</returns>
        /// <response code="200">The request was successful, and the product was updated.</response>
        [HttpPut("")]
        //[AuthorizeRoles(UserGroup.Board)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProduct(UpdateProductRequest product)
        {
            return Ok(await _productService.UpdateProduct(product));
        }
    }
}