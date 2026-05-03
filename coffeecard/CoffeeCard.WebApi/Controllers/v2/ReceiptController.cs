using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Receipts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2;

/// <summary>
/// Endpoints for retrieving the authenticated user's receipts.
/// Receipts include completed purchases, redeemed vouchers, and swiped tickets.
/// </summary>
[ApiVersion("2")]
[Route("api/v{version:apiVersion}/receipts")]
[ApiController]
[Authorize]
public class ReceiptController : ControllerBase
{
    private readonly IReceiptService _receiptService;
    private readonly ClaimsUtilities _claimsUtilities;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReceiptController"/> class.
    /// </summary>
    /// <param name="receiptService">Receipt service.</param>
    /// <param name="claimsUtilities">Helper for resolving the authenticated user from claims.</param>
    public ReceiptController(IReceiptService receiptService, ClaimsUtilities claimsUtilities)
    {
        _receiptService = receiptService;
        _claimsUtilities = claimsUtilities;
    }

    /// <summary>
    /// Retrieve a flat list of receipts for the authenticated user.
    /// Pass <c>type</c> to filter by receipt kind; omit it (or pass <c>All</c>) to get every type
    /// merged into a single list sorted by event date descending.
    /// </summary>
    /// <param name="request">Query parameters specifying the optional type filter.</param>
    /// <response code="200">The matching receipts, sorted newest-first.</response>
    /// <response code="401">Invalid or missing authentication credentials.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ReceiptsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReceiptsResponse>> GetReceipts(
        [FromQuery] ReceiptsRequest request
    )
    {
        var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
        var result = await _receiptService.GetReceipts(request.Type, user.Id);
        return Ok(result);
    }
}
