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
            return Created("register", new { message = "Your user has been created! Please check your email to verify your account.\n(Check your spam folder!)" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login(LoginDTO loginDto)
        {
            var token = _service.Login(loginDto.Email, loginDto.Password, loginDto.Version);
            if(token == null) return Unauthorized();
            return Ok(new { token = token });
        }
    }
}