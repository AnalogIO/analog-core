using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Helpers;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IMapperService _mapperService;
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService, IMapperService mapperService)
        {
            _ticketService = ticketService;
            _mapperService = mapperService;
        }

        /// <summary>
        ///     Returns a list of tickets. Use 'used' parameter to define what kind of tickets are returned
        /// </summary>
        [HttpGet]
        public IActionResult Get(bool used)
        {
            var tickets = _ticketService.getTickets(User.Claims, used);
            return Ok(_mapperService.Map(tickets).OrderBy(t => t.DateUsed).ToList());
        }

        /// <summary>
        ///     Uses the tickets supplied via ticketIds in the body
        /// </summary>
        [HttpPost("useMultiple")]
        public IActionResult UseMultipleTickets([FromBody] UseMultipleTicketDTO dto)
        {
            var usedTickets = _ticketService.UseMultipleTickets(User.Claims, dto);
            return Ok(_mapperService.Map(usedTickets));
        }


        /// <summary>
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns
        [HttpPost("use")]
        public IActionResult Use(UseTicketDTO dto)
        {
            var usedTicket = _ticketService.UseTicket(User.Claims, dto.ProductId);
            return Ok(_mapperService.Map(usedTicket));
        }
    }
}