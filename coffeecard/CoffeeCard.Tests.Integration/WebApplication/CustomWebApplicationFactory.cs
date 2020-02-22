using CoffeeCard.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Tests.Integration.WebApplication
{
	// Based on https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2

	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			// Use TestAppSettings
			builder.ConfigureAppConfiguration(configuration =>
			{
				configuration.AddJsonFile("appsettings-for-tests.json");
			});

			builder.ConfigureServices(services =>
			{
				// Create a new service provider.
				var serviceProvider = new ServiceCollection()
					.AddEntityFrameworkInMemoryDatabase()
					.BuildServiceProvider();

				// Add a database context (ApplicationDbContext) using an in-memory database for testing.
				services.AddDbContext<CoffeecardContext>(options =>
				{
					options.UseInMemoryDatabase("InMemoryDbForTesting");
					options.UseInternalServiceProvider(serviceProvider);
				});

				// Build the service provider
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database context (ApplicationDbContext).
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<CoffeecardContext>();
					var logger = scopedServices
						.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

					// Ensure the database is created.
					db.Database.EnsureCreated();
				}
			});
		}
	}
}
