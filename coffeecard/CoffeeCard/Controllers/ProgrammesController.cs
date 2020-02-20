using System.Linq;
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
        public IActionResult Get()
        {
            var programmes = _programmeService.GetProgrammes();
            return Ok(_mapperService.Map(programmes.OrderBy(x => x.SortPriority)).ToList());
        }
    }
}