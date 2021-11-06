using System;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;

namespace CoffeeCard.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
            {
                Log.Information("1");
                Log.Information("2");
                Log.Information("3");
                
                return Ok("pong");
            }
        }
    }
}