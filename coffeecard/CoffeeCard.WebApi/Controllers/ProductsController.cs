using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ClaimsUtilities _claimsUtilities;
        private readonly IMapperService _mapperService;
        private readonly IProductService _productService;

        public ProductsController(IProductService productService, IMapperService mapper,
            ClaimsUtilities claimsUtilities)
        {
            _productService = productService;
            _mapperService = mapper;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        ///     Returns a list of products
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProductsPublic()
        {
            var products = await _productService.GetPublicProducts();
            return Ok(_mapperService.Map(products).ToList());
        }

        [HttpGet("app")]
        public async Task<IActionResult> GetProductsForUser()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);

            var products = await _productService.GetProductsForUserAsync(user);
            return Ok(_mapperService.Map(products));
        }
    }
}