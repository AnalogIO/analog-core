using System.Net;
using System.Threading.Tasks;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers
{
    public class PingControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        public PingControllerTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;

        [Fact(DisplayName = "Ping returns HTTP 200 OK")]
        public async Task PingReturnsHttp200Ok()
        {
            // Arrange
            var client = _webApplicationFactory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/Ping");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "Ping returns pong")]
        public async Task PingReturnsPong()
        {
            // Arrange
            var client = _webApplicationFactory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/Ping");

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("pong", content);
        }
    }
}