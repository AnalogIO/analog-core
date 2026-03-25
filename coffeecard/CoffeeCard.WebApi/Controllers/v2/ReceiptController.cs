using System;
using System.Buffers.Text;
using System.Globalization;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.Receipts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2;

[ApiVersion("2")]
[Route("api/v{version:apiVersion}/receipts")]
[ApiController]
[Authorize]
public class ReceiptController : ControllerBase
{
    private readonly IReceiptService _receiptService;
    private readonly ClaimsUtilities _claimsUtilities;

    /// <summary>
    /// Contains endpoints for retrieving receipts for the authenticated user. This includes all purchases, swiped tickets, and used vouchers
    /// </summary>
    /// <param name="receiptService"></param>
    /// <param name="claimsUtilities"></param>
    public ReceiptController(IReceiptService receiptService, ClaimsUtilities claimsUtilities)
    {
        _receiptService = receiptService;
        _claimsUtilities = claimsUtilities;
    }

    /// <summary>
    /// Retrieve all receipts for the authenticated user
    /// This includes all purchases, swiped tickets, and used vouchers
    /// </summary>
    /// <returns>All users receipts</returns>
    public async Task<ActionResult<ReceiptResponse>> GetReceipts(
        [FromQuery] ReceiptsRequest request
    )
    {
        var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);

        DateTime from;
        if (request.ContinueationToken == null)
        {
            from = DateTime.UtcNow;
        }
        else
        {
            try
            {
                var bytes = Convert.FromBase64String(request.ContinueationToken);
                var utf8String = System.Text.Encoding.UTF8.GetString(bytes);
                from = DateTime.ParseExact(
                    utf8String,
                    "O",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind
                );
            }
            catch (FormatException)
            {
                throw new BadRequestException("Invalid continuation token");
            }
        }
        var receipts = await _receiptService.GetReceipts(
            from,
            request.BatchSize,
            request.Type,
            user.Id
        );

        return Ok(receipts);
    }
}
