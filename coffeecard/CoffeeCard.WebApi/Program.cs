using CoffeeCard.Common.Configuration;
using CoffeeCard.Library.Persistence;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.Library.Utils;
using CoffeeCard.WebApi.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CoffeeCard.WebApi
{
#pragma warning disable CS1591
    public class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithEnrichers()
                .CreateLogger();
            try
            {
                Log.Information("Starting web host");
                IHost webhost = CreateHostBuilder(args).Build();

                await PreStartupTasks(webhost);

                await webhost.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(((context, builder) =>
                {
                    _ = builder.AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);
                    _ = builder.AddEnvironmentVariables();
                }))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    _ = webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
        }

        private static async Task PreStartupTasks(IHost webhost)
        {
            using IServiceScope serviceScope = webhost.Services.CreateScope();
            EnvironmentSettings environment = serviceScope.ServiceProvider.GetRequiredService<EnvironmentSettings>();
            IFeatureManager featureManager = serviceScope.ServiceProvider.GetRequiredService<IFeatureManager>();

            Log.Information("Apply Database Migrations if any");
            await using CoffeeCardContext context = serviceScope.ServiceProvider.GetRequiredService<CoffeeCardContext>();
            if (context.Database.IsRelational())
            {
                context.Database.Migrate();
            }

            bool isMobilePayWebhookRegistrationManagementEnabled = await featureManager.IsEnabledAsync(FeatureFlags.MobilePayManageWebhookRegistration);
            Log.Information("FeatureFlag {flag} has enablement state '{value}'", nameof(FeatureFlags.MobilePayManageWebhookRegistration), isMobilePayWebhookRegistrationManagementEnabled);

            if (environment.EnvironmentType != EnvironmentType.LocalDevelopment && isMobilePayWebhookRegistrationManagementEnabled)
            {
                IWebhookService webhookService = serviceScope.ServiceProvider.GetRequiredService<IWebhookService>();
                await webhookService.EnsureWebhookIsRegistered();
            }
        }

        protected Program()
        {
        }
    }
#pragma warning restore CS1591
}