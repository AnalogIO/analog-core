using System;
using System.IO;
using System.Threading.Tasks;
using CoffeeCard.Library.Services.v2;
using CoffeeCard.WebApi.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

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
                var webhost = CreateHostBuilder(args).Build();

                var webhookService = webhost.Services.GetRequiredService<IWebhookService>();
                await webhookService.EnsureWebhookIsRegistered();
                
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
                    builder.AddJsonFile("appsettings.json", false, true);
                    builder.AddEnvironmentVariables();
                }))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
        }

        protected Program()
        {
        }
    }
#pragma warning restore CS1591
}