using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.DataTransferObjects.v2.Voucher;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for issuing, and redeeming vouchers
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/vouchers")]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly IPurchaseService _purchaseService;
        private readonly ClaimsUtilities _claimUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="VouchersController"/> class.
        /// </summary>
        /// <param name="voucherService">The voucher service</param>
        /// <param name="purchaseService">The purchase service</param>
        /// <param name="claimUtilities">The claims utilities</param>
        public VouchersController(
            IVoucherService voucherService,
            IPurchaseService purchaseService,
            ClaimsUtilities claimUtilities
        )
        {
            _voucherService = voucherService;
            _purchaseService = purchaseService;
            _claimUtilities = claimUtilities;
        }

        /// <summary>
        /// Issue voucher codes, that can later be redeemed
        /// </summary>
        /// <param name="request">Use ticket request</param>
        /// <returns>A list of voucher codes</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Bad Request. See explanation</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">Invalid role in credentials</response>
        [ProducesResponseType(typeof(IEnumerable<IssueVoucherResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [AuthorizeRoles(UserGroup.Board, UserGroup.Manager)]
        [HttpPost("issue-vouchers")]
        public async Task<ActionResult<IEnumerable<IssueVoucherResponse>>> IssueVouchers(
            [FromBody] IssueVoucherRequest request
        )
        {
            return Ok(await _voucherService.CreateVouchers(request));
        }

        /// <summary>
        /// Redeems the voucher supplied as parameter in the path
        /// </summary>
        /// <returns>Purchase description</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Voucher code already used</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">Voucher code not found</response>
        [HttpPost("{voucher-code}/redeem")]
        [ProducesResponseType(typeof(SimplePurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SimplePurchaseResponse>> RedeemVoucher(
            [FromRoute(Name = "voucher-code")] string voucherCode
        )
        {
            var user = await _claimUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            return Ok(await _purchaseService.RedeemVoucher(voucherCode, user));
        }
    }
}
