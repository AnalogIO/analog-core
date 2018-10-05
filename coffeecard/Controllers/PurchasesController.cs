using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using coffeecard.Services;
using Coffeecard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coffeecard.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        IPurchaseService _service;

        public PurchasesController(IPurchaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Purchase>> Get()
        {
            return _service.Read();
        }


        [HttpGet]
        public ActionResult<Purchase> Get(int id)
        {
            return _service.Read(id);
        }
    }
}