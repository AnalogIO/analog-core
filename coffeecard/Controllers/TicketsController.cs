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

        [HttpPut("use")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult UseTicket(UseTicketDTO ticket)
        {
            var usedTicket = _ticketService.UseTicket(User.Claims, ticket);
            return Ok(_mapperService.Map(usedTicket));
        }

        [HttpPut("usemultiple")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public ActionResult UseMultipleTickets(UseMultipleTicketDTO tickets)
        {
            var usedTickets = _ticketService.UseMultipleTickets(User.Claims, tickets);
            return Ok(_mapperService.Map(usedTickets));
        }
    }
}
