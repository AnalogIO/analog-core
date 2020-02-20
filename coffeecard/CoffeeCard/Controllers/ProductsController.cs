using System.Linq;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMapperService _mapperService;
        private readonly IProductService _productService;

        public ProductsController(IProductService productService, IMapperService mapper)
        {
            _productService = productService;
            _mapperService = mapper;
        }

        /// <summary>
        ///     Returns a list of products
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var products = _productService.GetProducts();
            return Ok(_mapperService.Map(products).ToList());
        }
    }
}