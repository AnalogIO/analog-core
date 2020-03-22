using System.Threading.Tasks;
using CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class MobilePayController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public MobilePayController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        ///     Initiates a purchase from the given productId and returns an orderId
        /// </summary>
        [HttpPost("initiate")]
        public IActionResult InitiatePurchase(InitiatePurchaseDTO initiatePurchaseDto)
        {
            var orderId = _purchaseService.InitiatePurchase(initiatePurchaseDto.ProductId, User.Claims);
            return Ok(new InitiatePurchaseResponseDTO {OrderId = orderId});
        }

        /// <summary>
        ///     Validates the purchase against mobilepay backend and delivers the tickets if succeeded
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> CompletePurchase(CompletePurchaseDTO dto)
        {
            await _purchaseService.CompletePurchase(dto, User.Claims);
            return Ok(new {Message = "The purchase was completed with success!"});
        }
    }
}