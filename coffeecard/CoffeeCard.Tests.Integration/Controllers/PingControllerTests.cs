using System.Net;
using System.Threading.Tasks;
using CoffeeCard.Tests.Integration.WebApplication;
using CoffeeCard.WebApi;
using Xunit;

namespace CoffeeCard.Tests.Integration.Controllers
{
	public class PingControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
	{
		private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;

		public PingControllerTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
		{
			_webApplicationFactory = webApplicationFactory;
		}

		[Fact(DisplayName = "Ping returns HTTP 200 OK")]
		public async Task PingReturnsHTTP200Ok()
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
