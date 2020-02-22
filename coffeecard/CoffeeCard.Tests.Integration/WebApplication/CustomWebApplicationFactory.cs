using CoffeeCard.Configuration;
using System.Linq;
using CoffeeCard.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCard.Tests.Integration.WebApplication
{
	// Based on https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1

	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
	{
        private IConfiguration Configuration { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			// Use TestAppSettings
			builder.ConfigureAppConfiguration(configuration =>
            {
				configuration.AddJsonFile("appsettings-for-tests.json");
                Configuration = configuration.Build();
            });

			builder.ConfigureServices(services =>
			{
                services.UseConfigurationValidation();

                // Parse and setup settings from configuration
                services.ConfigureValidatableSetting<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
                services.ConfigureValidatableSetting<EnvironmentSettings>(Configuration.GetSection("EnvironmentSettings"));
                services.ConfigureValidatableSetting<IdentitySettings>(Configuration.GetSection("IdentitySettings"));
                services.ConfigureValidatableSetting<MailgunSettings>(Configuration.GetSection("MailgunSettings"));
                services.ConfigureValidatableSetting<MobilePaySettings>(Configuration.GetSection("MobilePaySettings"));

				// Remove the app's ApplicationDbContext registration.
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType ==
					     typeof(DbContextOptions<CoffeeCardContext>));

				if (descriptor != null)
				{
					services.Remove(descriptor);
				}

				// Add ApplicationDbContext using an in-memory database for testing.
				services.AddDbContext<CoffeeCardContext>(options =>
				{
					options.UseInMemoryDatabase("InMemoryDbForTesting");
				});

				// Build the service provider
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database context (ApplicationDbContext).
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<CoffeeCardContext>();

                    // Ensure the database is created.
					db.Database.EnsureCreated();
				}
			});
		}
	}
}
