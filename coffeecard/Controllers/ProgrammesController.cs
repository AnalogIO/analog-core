using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Models.DataTransferObjects.Programme;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProgrammesController : ControllerBase
    {
        private readonly IMapperService _mapperService;
        private readonly IProgrammeService _programmeService;

        public ProgrammesController(IMapperService mapper, IProgrammeService programmeService)
        {
            _mapperService = mapper;
            _programmeService = programmeService;
        }

        /// <summary>
        ///     Returns a list of available programmes
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<ProgrammeDTO>> Get()
        {
            var programmes = _programmeService.GetProgrammes();
            return _mapperService.Map(programmes.OrderBy(x => x.SortPriority)).ToList();
        }
    }
}