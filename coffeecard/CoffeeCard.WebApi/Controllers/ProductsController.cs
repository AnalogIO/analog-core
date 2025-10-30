using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.Product;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for retrieving products
    /// </summary>
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ClaimsUtilities _claimsUtilities;
        private readonly IMapperService _mapperService;
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(
            IProductService productService,
            IMapperService mapper,
            ClaimsUtilities claimsUtilities
        )
        {
            _productService = productService;
            _mapperService = mapper;
            _claimsUtilities = claimsUtilities;
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
                products = await _productService.GetPublicProducts();
            }

            return Ok(_mapperService.Map(products).ToList());
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
            return Ok(_mapperService.Map(products));
        }
    }
}
