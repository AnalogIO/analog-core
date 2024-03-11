using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CoffeeCard.Tests.Integration.WebApplication
{
    // Based on https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private IConfiguration Configuration { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Use TestAppSettings
            _ = builder.ConfigureAppConfiguration(configuration =>
            {
                _ = configuration.AddJsonFile("appsettings.json");
                Configuration = configuration.Build();
            });

            _ = builder.ConfigureServices(services =>
            {
                _ = services.UseConfigurationValidation();

                // Parse and setup settings from configuration
                _ = services.ConfigureValidatableSetting<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));
                _ = services.ConfigureValidatableSetting<EnvironmentSettings>(
                    Configuration.GetSection("EnvironmentSettings"));
                _ = services.ConfigureValidatableSetting<IdentitySettings>(Configuration.GetSection("IdentitySettings"));
                _ = services.ConfigureValidatableSetting<MailgunSettings>(Configuration.GetSection("MailgunSettings"));

                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<CoffeeCardContext>));

                if (descriptor != null) _ = services.Remove(descriptor);

                // Add ApplicationDbContext using an in-memory database for testing.
                _ = services.AddDbContext<CoffeeCardContext>(options =>
                {
                    _ = options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

            });
        }
    }
}