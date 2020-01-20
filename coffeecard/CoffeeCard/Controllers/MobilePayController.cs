using System.Threading.Tasks;
using CoffeeCard.Helpers;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class MobilePayController : ControllerBase
    {
        private IMapperService _mapperService;
        private IMobilePayService _mobilePayService;
        private readonly IPurchaseService _purchaseService;

        public MobilePayController(IMobilePayService mobilePayService, IPurchaseService purchaseService,
            IMapperService mapperService)
        {
            _mobilePayService = mobilePayService;
            _purchaseService = purchaseService;
            _mapperService = mapperService;
        }

        /// <summary>
        ///     Initiates a purchase from the given productId and returns an orderId
        /// </summary>
        [HttpPost("initiate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public ActionResult<InitiatePurchaseResponseDTO> InitiatePurchase(InitiatePurchaseDTO initiatePurchaseDto)
        {
            var orderId = _purchaseService.InitiatePurchase(initiatePurchaseDto.ProductId, User.Claims);
            return Ok(new InitiatePurchaseResponseDTO {OrderId = orderId});
        }

        /// <summary>
        ///     Validates the purchase against mobilepay backend and delivers the tickets if succeeded
        /// </summary>
        [HttpPost("complete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<ActionResult<OkResult>> CompletePurchase(CompletePurchaseDTO dto)
        {
            var purchase = await _purchaseService.CompletePurchase(dto, User.Claims);
            return Ok(new {Message = "The purchase was completed with success!"});
        }
    }
}