using coffeecard.Models.DataTransferObjects.Ticket;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        ITicketService _ticketService;
        IMapperService _mapperService;

        public TicketsController(ITicketService ticketService, IMapperService mapperService)
        {
            _ticketService = ticketService;
            _mapperService = mapperService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public ActionResult<IEnumerable<TicketDTO>> Get(bool used)
        {
            var tickets = _ticketService.getTickets(User.Claims, used);
            return _mapperService.Map(tickets).ToList();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult UseTicket(int ticketId)
        {
            var usedTicket = _ticketService.UseTicket(User.Claims, ticketId);
            return Ok(_mapperService.Map(usedTicket));
        }

        [HttpPut("usemultiple")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult UseMultipleTickets(int[] ticketIds)
        {
            var usedTickets = _ticketService.UseMultipleTickets(User.Claims, ticketIds);
            return Ok(_mapperService.Map(usedTickets));
        }
    }
}
