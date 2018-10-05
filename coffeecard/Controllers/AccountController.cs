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
    public class AccountController : ControllerBase
    {
        IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register(RegisterDTO registerDto)
        {
            var user = _service.RegisterAccount(registerDto);
            return CreatedAtRoute("Register", new { id = user.Id }, user);
        }
    }
}