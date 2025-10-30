using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
using Serilog.Sinks.OpenTelemetry;

namespace CoffeeCard.WebApi
{
    /// <summary>
    /// The main entry point for the CoffeeCard.WebApi application.
    /// </summary>
    public class Program
    {
        private static IConfiguration Configuration { get; } =
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

        /// <summary>
        /// The entry point for the CoffeeCard.WebApi application.
        /// </summary>
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.WithEnrichers();

            var otlpSettings = Configuration.GetSection("OtlpSettings").Get<OtlpSettings>();
            var environment = Configuration
                .GetSection("EnvironmentSettings")
                .Get<EnvironmentSettings>();
            if (otlpSettings is not null)
            {
                var otlpExportProtocol = otlpSettings.Protocol switch
                {
                    OtelProtocol.Grpc => OtlpProtocol.Grpc,
                    OtelProtocol.Http => OtlpProtocol.HttpProtobuf,
                    _ => throw new ArgumentOutOfRangeException("Unspecified protocol for export"),
                };
                var resource = new Dictionary<string, object>();
                resource.Add("Env", environment.EnvironmentType.ToString() ?? "Env not set");
                resource.Add("service.name", $"analog-core-{environment.EnvironmentType}");
                loggerConfiguration.WriteTo.OpenTelemetry(settings =>
                {
                    settings.ResourceAttributes = resource;
                    settings.Endpoint = otlpSettings.Endpoint;
                    settings.Protocol = otlpExportProtocol;
                    settings.Headers.Add("Authorization", $"Basic {otlpSettings.Token}");
                });
            }

            Log.Logger = loggerConfiguration.CreateLogger();
            try
            {
                Log.Information("Starting web host");
                var webhost = CreateHostBuilder(args).Build();

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
                .ConfigureAppConfiguration(
                    (
                        (context, builder) =>
                        {
                            builder.AddJsonFile(
                                path: "appsettings.json",
                                optional: false,
                                reloadOnChange: true
                            );
                            builder.AddEnvironmentVariables();
                        }
                    )
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
        }

        private static async Task PreStartupTasks(IHost webhost)
        {
            using var serviceScope = webhost.Services.CreateScope();
            var environment =
                serviceScope.ServiceProvider.GetRequiredService<EnvironmentSettings>();
            var featureManager = serviceScope.ServiceProvider.GetRequiredService<IFeatureManager>();

            Log.Information("Apply Database Migrations if any");
            await using var context =
                serviceScope.ServiceProvider.GetRequiredService<CoffeeCardContext>();
            if (context.Database.IsRelational())
            {
                context.Database.Migrate();
            }

            var isMobilePayWebhookRegistrationManagementEnabled =
                await featureManager.IsEnabledAsync(
                    FeatureFlags.MobilePayManageWebhookRegistration
                );
            Log.Information(
                "FeatureFlag {flag} has enablement state '{value}'",
                nameof(FeatureFlags.MobilePayManageWebhookRegistration),
                isMobilePayWebhookRegistrationManagementEnabled
            );

            if (
                environment.EnvironmentType != EnvironmentType.LocalDevelopment
                && isMobilePayWebhookRegistrationManagementEnabled
            )
            {
                var webhookService =
                    serviceScope.ServiceProvider.GetRequiredService<IWebhookService>();
                await webhookService.EnsureWebhookIsRegistered();
            }
        }

        protected Program() { }
    }
}
