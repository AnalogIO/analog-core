using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects;
using CoffeeCard.Models.DataTransferObjects.v2.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ITicketService = CoffeeCard.Library.Services.v2.ITicketService;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for retrieving and using tickets
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ClaimsUtilities _claimsUtilities;
        private readonly ITicketService _ticketService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketsController"/> class.
        /// </summary>
        public TicketsController(ITicketService ticketService, ClaimsUtilities claimsUtilities)
        {
            _ticketService = ticketService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Returns a list of tickets
        /// </summary>
        /// <param name="includeUsed">Include already used tickets</param>
        /// <returns>List of tickets</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TicketResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TicketResponse>>> Get([FromQuery] bool includeUsed)
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);

            return Ok(await _ticketService.GetTickets(user, includeUsed));
        }

        /// <summary>
        /// Uses a ticket (for the given product) on the given menu item
        /// </summary>
        /// <param name="productId">The id of the product the ticket is for</param>
        /// <param name="menuItemId">The id of the menu item to use the ticket on</param>
        /// <returns>The ticket that was used</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="403">User has no tickets for the product or the menu item is not eligible for the ticket</response>
        /// <response code="404">The product or menu item could not be found</response>
        [ProducesResponseType(typeof(UsedTicketResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
        [HttpPost("use/{productId:int}/{menuItemId:int}")]
        public async Task<ActionResult<UsedTicketResponse>> UseTicket(int productId, int menuItemId)
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);

            return Ok(await _ticketService.UseTicketAsync(user, productId, menuItemId));
        }
    }
}