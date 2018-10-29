using coffeecard.Models.DataTransferObjects.Ticket;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/[controller]")]
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
    }
}
