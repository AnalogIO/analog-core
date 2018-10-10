using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        IAccountService _accountService;

        public PurchasesController(IPurchaseService purchaseService, IMapperService mapper, IAccountService accountService)
        {
            _purchaseService = purchaseService;
            _mapperService = mapper;
            _accountService = accountService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PurchaseDTO>> Get()
        {
            var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            if (emailClaim == null) return Unauthorized();
            var userId = _accountService.GetIdFromEmail(emailClaim.Value);
            return _mapperService.Map(_purchaseService.GetPurchases(userId)).ToList();
        }


        [HttpGet("{id}")]
        public ActionResult<PurchaseDTO> Get(int id)
        {
            return _mapperService.Map(_purchaseService.GetPurchase(id));
        }
    }
}