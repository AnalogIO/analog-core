using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.v2.Purchase;
using CoffeeCard.Models.Entities;
using CoffeeCard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for initiating and retrieving purchases
    /// </summary>
    [ApiController]
    [Authorize]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/purchases")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ClaimsUtilities _claimsUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchasesController"/> class.
        /// </summary>
        public PurchasesController(
            IPurchaseService purchaseService,
            ClaimsUtilities claimsUtilities
        )
        {
            _purchaseService = purchaseService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Get all purchases
        /// </summary>
        /// <returns>List of purchases</returns>
        /// <response code="200">Purchases</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SimplePurchaseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<SimplePurchaseResponse>>> GetAllPurchases()
        {
            var purchases = await _purchaseService.GetPurchases(
                await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims)
            );

            return Ok(purchases);
        }

        /// <summary>
        /// Get all purchases for a user
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>List of purchases</returns>
        /// <response code="200">Purchases</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">User not allowed to view purchases for given user</response>
        /// <response code="404">User with userId not found</response>
        [HttpGet("user/{userId}")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(List<SimplePurchaseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SimplePurchaseResponse>>> GetAllPurchasesForUser(
            [FromRoute] int userId
        )
        {
            var purchases = await _purchaseService.GetPurchases(userId);

            return Ok(purchases);
        }

        /// <summary>
        /// Get purchase
        /// </summary>
        /// <param name="id">Purchase Id</param>
        /// <returns>Purchase</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="404">No purchase found with purchase-id</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SinglePurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SinglePurchaseResponse>> GetPurchase([FromRoute] int id)
        {
            var purchase = await _purchaseService.GetPurchase(
                id,
                await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims)
            );

            return Ok(purchase);
        }

        /// <summary>
        /// Initiate a new payment.
        /// </summary>
        /// <param name="initiateRequest">Initiate request</param>
        /// <returns>Purchase with payment details</returns>
        /// <response code="200">Purchased initiated</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">User not allowed to purchase given product</response>
        [HttpPost]
        [ProducesResponseType(typeof(InitiatePurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<InitiatePurchaseResponse>> InitiatePurchase(
            [FromBody] InitiatePurchaseRequest initiateRequest
        )
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            var purchaseResponse = await _purchaseService.InitiatePurchase(initiateRequest, user);

            // TODO Return CreatedAtAction
            return Ok(purchaseResponse);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="id">database id of purchase</param>
        /// <returns>Purchase after being refunded</returns>
        [HttpPut("{id}/refund")]
        [AuthorizeRoles(UserGroup.Board)]
        [ProducesResponseType(typeof(SimplePurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> RefundPurchase([FromRoute] int id)
        {
            var refundResponse = await _purchaseService.RefundPurchase(id);
            return Ok(refundResponse);
        }
    }
}
