using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.v2.Programme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers.v2
{
    /// <summary>
    /// Controller for listing study programmes.
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/programmes")]
    public class ProgrammesController : ControllerBase
    {
        private readonly IProgrammeService _programmeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgrammesController"/> class.
        /// </summary>
        public ProgrammesController(IProgrammeService programmeService)
        {
            _programmeService = programmeService;
        }

        /// <summary>
        /// Returns the list of active study programmes.
        /// </summary>
        /// <returns>List of active study programmes, ordered by sort priority.</returns>
        /// <response code="200">Successful request</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ProgrammeResponse>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProgrammeResponse>> Get()
        {
            var programmes = _programmeService
                .GetProgrammes()
                .OrderBy(programme => programme.SortPriority)
                .Select(programme => new ProgrammeResponse
                {
                    Id = programme.Id,
                    ShortName = programme.ShortName,
                    FullName = programme.FullName,
                });

            return Ok(programmes);
        }
    }
}
