﻿using System.Collections.Generic;
using System.Linq;
using coffeecard.Models.DataTransferObjects.Product;
using coffeecard.Services;
using Microsoft.AspNetCore.Mvc;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService _productService;
        IMapperService _mapperService;

        public ProductsController(IProductService productService, IMapperService mapper)
        {
            _productService = productService;
            _mapperService = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<IEnumerable<ProductDTO>> Get()
        {
            var products = _productService.GetProducts();
            return Ok(_mapperService.Map(products).ToList());
        }
    }
}