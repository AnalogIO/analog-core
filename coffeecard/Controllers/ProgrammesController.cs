using coffeecard.Models.DataTransferObjects.Programme;
using coffeecard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace coffeecard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProgrammesController : ControllerBase
    {
        IMapperService _mapperService;
        IProgrammeService _programmeService;

        public ProgrammesController(IMapperService mapper, IProgrammeService programmeService)
        { 
            _mapperService = mapper;
            _programmeService = programmeService;
        }

        /// <summary>
        ///  Returns a list of available programmes
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
