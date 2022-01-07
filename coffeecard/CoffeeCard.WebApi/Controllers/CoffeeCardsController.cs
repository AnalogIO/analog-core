using System.Collections.Generic;
using CoffeeCard.Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<Common.Models.CoffeeCard>> Get()
        {
            return Ok(_ticketService.GetCoffeeCards(User.Claims));
        }
    }
}