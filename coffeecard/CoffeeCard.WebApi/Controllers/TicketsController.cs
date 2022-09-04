using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeCard.Common.Errors;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for retrieving and using tickets
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly IMapperService _mapperService;
        private readonly ITicketService _ticketService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketsController"/> class.
        /// </summary>
        public TicketsController(ITicketService ticketService, IMapperService mapperService)
        {
            _ticketService = ticketService;
            _mapperService = mapperService;
        }

        /// <summary>
        /// Returns a list of tickets
        /// </summary>
        /// <param name="used">Include already used tickets</param>
        /// <returns>List of tickets</returns>
        /// <response code="200">Successful request</response>
        /// <response code="401">Invalid credentials</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public ActionResult<List<TicketDto>> Get([FromQuery] bool used)
        {
            var tickets = _ticketService.GetTickets(User.Claims, used);
            return Ok(_mapperService.Map(tickets).OrderBy(t => t.DateUsed).ToList());
        }

        /// <summary>
        /// Uses the tickets supplied via product ids in the body
        /// </summary>
        /// <param name="dto">Use multiple tickets request</param>
        /// <returns>List of used tickets</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Bad Request, not enough tickets. See explanation</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("useMultiple")]
        [ProducesResponseType(typeof(List<TicketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TicketDto>>> UseMultipleTickets([FromBody] UseMultipleTicketDto dto)
        {
            var usedTickets = await _ticketService.UseMultipleTickets(User.Claims, dto);
            return Ok(_mapperService.Map(usedTickets));
        }
        
        /// <summary>
        /// Use ticket request
        /// </summary>
        /// <param name="dto">Use ticket request</param>
        /// <returns>Used ticket </returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Bad Request, not enough tickets. See explanation</response>
        /// <response code="401">Invalid credentials</response>
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [HttpPost("use")]
        public async Task<ActionResult<TicketDto>> Use([FromBody] UseTicketDTO dto)
        {
            var usedTicket = await _ticketService.UseTicket(User.Claims, dto.ProductId);
            return Ok(_mapperService.Map(usedTicket));
        }
    }
}