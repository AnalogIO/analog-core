﻿using CoffeeCard.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CoffeeCard.Tests.Unit.Controllers
{
    public class PingControllerTests
    {
        [Fact(DisplayName = "Ping returns OkObjectResult")]
        public void PingReturnsOkObjectResult()
        {
            var controller = new PingController();

            var result = controller.Ping();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact(DisplayName = "Ping returns pong")]
        public void PingReturnsPong()
        {
            var controller = new PingController();

            var result = (OkObjectResult) controller.Ping();

            Assert.Equal("pong", result.Value);
        }
    }
}