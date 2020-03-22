﻿using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class CoffeeCardsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public CoffeeCardsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_ticketService.GetCoffeeCards(User.Claims));
        }
    }
}