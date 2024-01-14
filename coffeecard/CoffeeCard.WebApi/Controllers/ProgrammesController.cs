using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Library.Services;
using CoffeeCard.Models.DataTransferObjects.Programme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    /// <summary>
    /// Controller for listing study programmes
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProgrammesController : ControllerBase
    {
        private readonly IMapperService _mapperService;
        private readonly IProgrammeService _programmeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgrammesController"/> class.
        /// </summary>
        public ProgrammesController(IMapperService mapper, IProgrammeService programmeService)
        {
            _mapperService = mapper;
            _programmeService = programmeService;
        }

        /// <summary>
        /// Returns a list of available programmes
        /// </summary>
        /// <returns>List of available programmes</returns>
        /// <response code="200">Successful request</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ProgrammeDto>), StatusCodes.Status200OK)]
        public ActionResult<List<ProgrammeDto>> Get()
        {
            var programmes = _programmeService.GetProgrammes();
            return Ok(_mapperService.Map(programmes.OrderBy(x => x.SortPriority)).ToList());
        }
    }
}
