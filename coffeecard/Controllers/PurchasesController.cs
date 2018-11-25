﻿using System.Collections.Generic;
using System.Linq;
using coffeecard.Models.DataTransferObjects.Purchase;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        IPurchaseService _purchaseService;
        IMapperService _mapperService;
        IAccountService _accountService;

        public PurchasesController(IPurchaseService purchaseService, IMapperService mapper, IAccountService accountService)
        {
            _purchaseService = purchaseService;
            _mapperService = mapper;
            _accountService = accountService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<IEnumerable<PurchaseDTO>> Get()
        {
            var purchases = _purchaseService.GetPurchases(User.Claims);
            return _mapperService.Map(purchases).ToList();
        }
    }
}