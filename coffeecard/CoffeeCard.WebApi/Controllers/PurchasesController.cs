using System;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    // TODO: Remove this Controller after June 2023

    /// <summary>
    /// Controller for retrieving and issuing
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IMapperService _mapperService;
        private readonly IPurchaseService _purchaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchasesController"/> class.
        /// </summary>
        public PurchasesController(IPurchaseService purchaseService, IMapperService mapper)
        {
            _purchaseService = purchaseService;
            _mapperService = mapper;
        }

        /// <summary>
        /// Returns a list of purchases for the given user via the supplied token in the header
        /// </summary>
        /// <returns>List of purchases</returns>
        /// <response code="410">Deprecated</response>
        [HttpGet]
        [Obsolete(message: "Replaced by Purchases API v2")]
        [ProducesResponseType(typeof(void), StatusCodes.Status410Gone)]
        public ActionResult Get()
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        /// <summary>
        /// Redeems the voucher supplied as parameter in the path
        /// </summary>
        /// <returns>Purchase description</returns>
        /// <response code="200">Successful request</response>
        /// <response code="409">Voucher code already used</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">Voucher code not found</response>
        [HttpPost("redeemvoucher")]
        [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PurchaseDto>> RedeemVoucher([FromQuery] string voucherCode)
        {
            var purchase = await _purchaseService.RedeemVoucher(voucherCode, User.Claims);
            return Ok(_mapperService.Map(purchase));
        }

        /// <summary>
        /// Issue purchase used by the ipad in the cafe
        /// </summary>
        /// <returns>Purchase description</returns>
        /// <param name="issueProduct"></param>
        /// <response code="410">Deprecated</response>
        [HttpPost("issueproduct")]
        [AllowAnonymous]
        [Obsolete(message: "Replaced by Purchases API v2")]
        [ProducesResponseType(typeof(void), StatusCodes.Status410Gone)]
        public ActionResult IssueProduct([FromBody] IssueProductDto issueProduct)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }
}
