using System;
using CoffeeCard.Models.DataTransferObjects.MobilePay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    // TODO: Remove this Controller after June 2023

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
        public MobilePayController() { }

        /// <summary>
        /// Initiates a purchase from the given productId and returns an orderId
        /// </summary>
        /// <param name="initiatePurchaseDto">Initiate purchase request</param>
        /// <returns>Response with order id</returns>
        /// <response code="410">Deprecated</response>
        [HttpPost("initiate")]
        [Obsolete(message: "Replaced by Purchases API v2")]
        [ProducesResponseType(typeof(void), StatusCodes.Status410Gone)]
        public ActionResult InitiatePurchase([FromBody] InitiatePurchaseDto initiatePurchaseDto)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        /// <summary>
        /// Validates the purchase against MobilePay and delivers the tickets if succeeded
        /// </summary>
        /// <param name="dto">Complete purchase request with MobilePay reference</param>
        /// <response code="410">Deprecated</response>
        [HttpPost("complete")]
        [Obsolete(message: "Replaced by Purchases API v2")]
        [ProducesResponseType(typeof(void), StatusCodes.Status410Gone)]
        public ActionResult CompletePurchase([FromBody] CompletePurchaseDto dto)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}
