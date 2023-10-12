using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.Product;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for retrieving products
    /// </summary>
    [Authorize]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ClaimsUtilities _claimsUtilities;
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(IProductService productService,
            ClaimsUtilities claimsUtilities)
        {
            _productService = productService;
            _claimsUtilities = claimsUtilities;
        }

        private ProductDto mapProductToDto(Product product)
        {
            return new ProductDto
            {
                Description = product.Description,
                Id = product.Id,
                Name = product.Name,
                NumberOfTickets = product.NumberOfTickets,
                Price = product.Price,
                IsPerk = product.isPerk()
            };
        }

        /// <summary>
        /// Returns a list of available products based on a account's user group
        /// </summary>
        /// <returns>List of available products</returns>
        /// <response code="200">Successful request</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductDto>>> GetProductsPublic()
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


            return Ok(products.Select(mapProductToDto).ToList());
        }

        /// <summary>
        /// Returns a list of available products based on a account's user group
        /// </summary>
        /// <returns>List of available product</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet("app")]
        [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ProductDto>>> GetProductsForUser()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);

            var products = await _productService.GetProductsForUserAsync(user);
            return Ok(products.Select(mapProductToDto));
        }
    }
}