using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CoffeeCard.Configuration;
using CoffeeCard.Console.Refund.Handler;
using CoffeeCard.Console.Refund.IO;
using CoffeeCard.Console.Refund.Model;
using CoffeeCard.Helpers.MobilePay;
using CoffeeCard.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.Console.Refund
{
    class Program
    {
        private static ServiceProvider _serviceProvider;
        private static ILogger _log;
        
        static async Task Main(string[] args)
        {
            Startup(new ServiceCollection());
            await RefundPayments("input.txt");

            _log.LogInformation("Finished processing refunds");
        }

        static void Startup(IServiceCollection services)
        {
            // Load Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // setup DI for MobilePay services
            services
                .AddLogging(conf => conf.AddConsole())
                .AddScoped<IInputReader<CompletedOrder>, CompletedOrderInputReader>()
                .AddScoped<IOutputWriter<IList<RefundResponse>>, MobilePayRefundOutputWriter>()
                .AddScoped<IMobilePayService, MobilePayService>()
                .AddScoped<RefundHandler>()
                .AddHttpClient<IMobilePayApiHttpClient, MobilePayApiHttpClient>();

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);

            // Load MobilePaySettings from Configuration
            services.UseConfigurationValidation();
            services.ConfigureValidatableSetting<MobilePaySettings>(configuration.GetSection("MobilePaySettings"));
            
            _serviceProvider = services.BuildServiceProvider();

            _log = _serviceProvider.GetService<ILogger<Program>>();

            _log.LogInformation("Dependency Injection and configuration setup");
        }

        private static async Task RefundPayments(string path)
        {
            var mpInputReader = _serviceProvider.GetService<IInputReader<CompletedOrder>>();
            var completedOrders = await mpInputReader.ReadFromCommaSeparatedFile(path);

            await _serviceProvider.GetService<RefundHandler>().RefundPayments(completedOrders);
        }
    }
}
