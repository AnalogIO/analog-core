using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeecard.Models.DataTransferObjects.Purchase;
using coffeecard.Services;
using Coffeecard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coffeecard.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        IPurchaseService _purchaseService;
        IMapperService _mapperService;

        public PurchasesController(IPurchaseService service, IMapperService mapper)
        {
            _purchaseService = service;
            _mapperService = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PurchaseDTO>> Get()
        {
            return _mapperService.Map(_purchaseService.Read()).ToList();
        }


        [HttpGet("{id}")]
        public ActionResult<PurchaseDTO> Get(int id)
        {
            return _mapperService.Map(_purchaseService.Read(id));
        }
    }
}