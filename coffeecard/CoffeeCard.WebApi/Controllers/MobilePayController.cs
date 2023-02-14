using System.Threading.Tasks;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for initiating and completing a purchase with MobilePay
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class MobilePayController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayController"/> class.
        /// </summary>
        public MobilePayController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Initiates a purchase from the given productId and returns an orderId
        /// </summary>
        /// <param name="initiatePurchaseDto">Initiate purchase request</param>
        /// <returns>Response with order id</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("initiate")]
        [ProducesResponseType(typeof(InitiatePurchaseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<InitiatePurchaseResponseDto> InitiatePurchase([FromBody] InitiatePurchaseDto initiatePurchaseDto)
        {
            var orderId = _purchaseService.InitiatePurchase(initiatePurchaseDto.ProductId, User.Claims);
            return Ok(new InitiatePurchaseResponseDto {OrderId = orderId});
        }

        /// <summary>
        /// Validates the purchase against MobilePay and delivers the tickets if succeeded
        /// </summary>
        /// <param name="dto">Complete purchase request with MobilePay reference</param>
        /// <response code="200">Purchase successfully fulfilled</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("complete")]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MessageResponseDto>> CompletePurchase([FromBody] CompletePurchaseDto dto)
        {
            await _purchaseService.CompletePurchase(dto, User.Claims);
            return Ok(new MessageResponseDto()
            {
                Message = "The purchase was completed with success!"
            });
        }
    }
}