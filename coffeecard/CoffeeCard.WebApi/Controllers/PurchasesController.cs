using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Common.Errors;
using CoffeeCard.Common.Models.DataTransferObjects.Purchase;
using CoffeeCard.Library.Services;
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
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<List<PurchaseDto>> Get()
        {
            var purchases = _purchaseService.GetPurchases(User.Claims);
            return Ok(_mapperService.Map(purchases).OrderBy(p => p.DateCreated).ToList());
        }

        /// <summary>
        /// Redeems the voucher supplied as parameter in the path
        /// </summary>
        [HttpPost("redeemvoucher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PurchaseDto> RedeemVoucher([FromQuery] string voucherCode)
        {
            var purchase = _purchaseService.RedeemVoucher(voucherCode, User.Claims);
            return Ok(_mapperService.Map(purchase));
        }

        /// <summary>
        /// Issue purchase used by the ipad in the cafe
        /// </summary>
        /// <param name="issueProduct"></param>
        /// <returns></returns>
        [HttpPost("issueproduct")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult IssueProduct([FromBody] IssueProductDto issueProduct)
        {
            var adminToken = Request.Headers.FirstOrDefault(x => x.Key == "admintoken").Value.FirstOrDefault();
            Log.Information(adminToken);
            if (adminToken != _identitySettings.AdminToken) throw new ApiException("AdminToken was invalid", 401);
            var purchase = _purchaseService.IssueProduct(issueProduct);
            return Ok(_mapperService.Map(purchase));
        }
    }
}