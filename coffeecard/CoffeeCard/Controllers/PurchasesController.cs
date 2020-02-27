using System.Linq;
using CoffeeCard.Common.Configuration;
using CoffeeCard.Helpers;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IdentitySettings _identitySettings;
        private readonly IMapperService _mapperService;
        private readonly IPurchaseService _purchaseService;
        private IAccountService _accountService;

        public PurchasesController(IPurchaseService purchaseService, IMapperService mapper,
            IAccountService accountService, IdentitySettings identitySettings)
        {
            _purchaseService = purchaseService;
            _mapperService = mapper;
            _accountService = accountService;
            _identitySettings = identitySettings;
        }

        /// <summary>
        ///     Returns a list of purchases for the given user via the supplied token in the header
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var purchases = _purchaseService.GetPurchases(User.Claims);
            return Ok(_mapperService.Map(purchases).OrderBy(p => p.DateCreated).ToList());
        }

        /// <summary>
        ///     Redeems the voucher supplied as parameter in the path
        /// </summary>
        [HttpPost("redeemvoucher")]
        public IActionResult RedeemVoucher(string voucherCode)
        {
            var purchase = _purchaseService.RedeemVoucher(voucherCode, User.Claims);
            return Ok(_mapperService.Map(purchase));
        }

        /// <summary>
        ///     Issue purchase used by the ipad in the cafe
        /// </summary>
        /// <param name="issueProduct"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("issueproduct")]
        public IActionResult IssueProduct(IssueProductDTO issueProduct)
        {
            var adminToken = Request.Headers.FirstOrDefault(x => x.Key == "admintoken").Value.FirstOrDefault();
            Log.Information(adminToken);
            if (adminToken != _identitySettings.AdminToken) throw new ApiException("AdminToken was invalid", 401);
            var purchase = _purchaseService.IssueProduct(issueProduct);
            return Ok(_mapperService.Map(purchase));
        }
    }
}