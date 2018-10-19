using coffeecard.Models.DataTransferObjects.Programme;
using coffeecard.Services;
using Coffeecard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeecard.Controllers
{
    [Route("api/[controller]")]
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<IEnumerable<ProgrammeDTO>> Get()
        {
            var programmes = _programmeService.GetProgrammes();
            return _mapperService.Map(programmes.OrderBy(x => x.SortPriority)).ToList();
        }
    }
}
