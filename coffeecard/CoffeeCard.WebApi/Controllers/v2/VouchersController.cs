using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
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
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/vouchers")]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
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
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [Authorize]
        [AuthorizeRoles(UserGroup.Board)]
        [HttpPost("issueVouchers")]
        public async Task<ActionResult<IEnumerable<IssueVoucherResponse>>> IssueVouchers([FromBody] IssueVoucherRequest request)
        {
            return Ok(await _voucherService.CreateVouchers(request));
        }
    }
}