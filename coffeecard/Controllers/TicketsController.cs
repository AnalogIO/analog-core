using coffeecard.Helpers;
using coffeecard.Models.DataTransferObjects.Ticket;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        ///  Returns a list of tickets. Use 'used' parameter to define what kind of tickets are returned
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<IEnumerable<TicketDTO>> Get(bool used)
        {
            var tickets = _ticketService.getTickets(User.Claims, used);
            return _mapperService.Map(tickets).ToList();
        }

        /// <summary>
        ///  Uses the ticket via the ticketId supplied as parameter
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<TicketDTO> UseTicket(int ticketId)
        {
            var usedTicket = _ticketService.UseTicket(User.Claims, ticketId);
            return Ok(_mapperService.Map(usedTicket));
        }

        /// <summary>
        ///  Uses the tickets supplied via ticketIds in the body
        /// </summary>
        [HttpPut("usemultiple")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<IEnumerable<TicketDTO>> UseMultipleTickets(int[] ticketIds)
        {
            var usedTickets = _ticketService.UseMultipleTickets(User.Claims, ticketIds);
            return Ok(_mapperService.Map(usedTickets));
        }
    }
}
