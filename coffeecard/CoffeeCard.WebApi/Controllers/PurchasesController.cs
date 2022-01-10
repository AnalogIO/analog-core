using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for retrieving and issuing
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IdentitySettings _identitySettings;
        private readonly IMapperService _mapperService;
        private readonly IPurchaseService _purchaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchasesController"/> class.
        /// </summary>
        public PurchasesController(IPurchaseService purchaseService, IMapperService mapper, IdentitySettings identitySettings)
        {
            _purchaseService = purchaseService;
            _mapperService = mapper;
            _identitySettings = identitySettings;
        }

        /// <summary>
        /// Returns a list of purchases for the given user via the supplied token in the header
        /// </summary>
        /// <returns>List of purchases</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<PurchaseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<List<PurchaseDto>> Get()
        {
            var purchases = _purchaseService.GetPurchases(User.Claims);
            return Ok(_mapperService.Map(purchases).OrderBy(p => p.DateCreated).ToList());
        }

        /// <summary>
        /// Redeems the voucher supplied as parameter in the path
        /// </summary>
        /// <returns>Purchase description</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Voucher code already used</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">Voucher code not found</response>
        [HttpPost("redeemvoucher")]
        [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status404NotFound)]
        public ActionResult<PurchaseDto> RedeemVoucher([FromQuery] string voucherCode)
        {
            var purchase = _purchaseService.RedeemVoucher(voucherCode, User.Claims);
            return Ok(_mapperService.Map(purchase));
        }

        /// <summary>
        /// Issue purchase used by the ipad in the cafe
        /// </summary>
        /// <returns>Purchase description</returns>
        /// <param name="issueProduct"></param>
        /// <returns></returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("issueproduct")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PurchaseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<PurchaseDto> IssueProduct([FromBody] IssueProductDto issueProduct)
        {
            var adminToken = Request.Headers.FirstOrDefault(x => x.Key == "admintoken").Value.FirstOrDefault();
            Log.Information(adminToken);
            if (adminToken != _identitySettings.AdminToken) throw new ApiException("AdminToken was invalid", StatusCodes.Status401Unauthorized);
            var purchase = _purchaseService.IssueProduct(issueProduct);
            return Ok(_mapperService.Map(purchase));
        }
    }
}