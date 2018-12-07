using coffeecard.Models.DataTransferObjects.MobilePay;
using coffeecard.Models.DataTransferObjects.Ticket;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class MobilePayController : ControllerBase
    {
        IMobilePayService _mobilePayService;
        IPurchaseService _purchaseService;
        IMapperService _mapperService;

        public MobilePayController(IMobilePayService mobilePayService, IPurchaseService purchaseService, IMapperService mapperService)
        {
            _mobilePayService = mobilePayService;
            _purchaseService = purchaseService;
            _mapperService = mapperService;
        }

        [HttpPost("initiatepurchase")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult InitiatePurchase(InitiatePurchaseDTO initiatePurchaseDto)
        {
            var orderId = _purchaseService.InitiatePurchase(initiatePurchaseDto.ProductId, User.Claims);
            return Ok(new { OrderId = orderId });
        }

        [HttpPost("completepurchase")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult CompletePurchase(CompletePurchaseDTO dto)
        {
            var purchase = _purchaseService.CompletePurchase(dto, User.Claims);
            return Ok(new { Message = "The purchase was completed with success!" });
        }

    }
}
