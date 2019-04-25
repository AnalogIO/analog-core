using coffeecard.Helpers;
using coffeecard.Services;
using Coffeecard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class CoffeeCardsController : ControllerBase
    {
        private ITicketService _ticketService;

        public CoffeeCardsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<IEnumerable<CoffeCard>> Get()
        {
            return Ok(_ticketService.GetCoffeCards(User.Claims));
        }
    }
}