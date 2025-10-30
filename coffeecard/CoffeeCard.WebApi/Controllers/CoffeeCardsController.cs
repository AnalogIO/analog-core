using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.CoffeeCard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    // TODO: Deprecate this controller and clean up subsequents services if no App is using this

    /// <summary>
    /// Controller for retrieving an account's coffee cards
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CoffeeCardsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeCardsController"/> class.
        /// </summary>
        public CoffeeCardsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Retrieve the coffee cards of the account
        /// </summary>
        /// <returns>Coffee cards</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(CoffeeCardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<CoffeeCardDto> Get()
        {
            return Ok(_ticketService.GetCoffeeCards(User.Claims));
        }
    }
}
