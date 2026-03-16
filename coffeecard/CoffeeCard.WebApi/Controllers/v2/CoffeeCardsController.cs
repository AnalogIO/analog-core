using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Library.Utils;
using CoffeeCard.Models.DataTransferObjects.v2.CoffeeCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ITicketService = CoffeeCard.Library.Services.v2.ITicketService;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for retrieving an account's coffee cards
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/coffeecards")]
    [Authorize]
    public class CoffeeCardsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ClaimsUtilities _claimsUtilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeCardsController"/> class.
        /// </summary>
        public CoffeeCardsController(ITicketService ticketService, ClaimsUtilities claimsUtilities)
        {
            _ticketService = ticketService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        /// Retrieve the coffee cards of the account
        /// </summary>
        /// <returns>Coffee cards</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CoffeeCardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CoffeeCardResponse>>> Get()
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            return Ok(await _ticketService.GetCoffeeCardsAsync(user));
        }
    }
}