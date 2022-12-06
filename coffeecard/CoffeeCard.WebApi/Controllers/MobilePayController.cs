using System;
using System.Threading.Tasks;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="MobilePayController"/> class.
        /// </summary>
        public MobilePayController()
        {
        }

        /// <summary>
        /// Initiates a purchase from the given productId and returns an orderId
        /// </summary>
        /// <param name="initiatePurchaseDto">Initiate purchase request</param>
        /// <returns>Response with order id</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("initiate")]
        [Obsolete(message: "Replaced by Purchases API v2")]
        [ProducesResponseType(typeof(InitiatePurchaseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<InitiatePurchaseResponseDto> InitiatePurchase([FromBody] InitiatePurchaseDto initiatePurchaseDto)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        /// <summary>
        /// Validates the purchase against MobilePay and delivers the tickets if succeeded
        /// </summary>
        /// <param name="dto">Complete purchase request with MobilePay reference</param>
        /// <response code="200">Purchase successfully fulfilled</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("complete")]
        [Obsolete(message: "Replaced by Purchases API v2")]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MessageResponseDto>> CompletePurchase([FromBody] CompletePurchaseDto dto)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}