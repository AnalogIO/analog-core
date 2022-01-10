using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Client;
using CoffeeCard.MobilePay.RefundConsoleApp.Handler;
using CoffeeCard.MobilePay.RefundConsoleApp.IO;
using CoffeeCard.MobilePay.RefundConsoleApp.Model;
using CoffeeCard.MobilePay.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.RefundConsoleApp
{
    internal class Program
    {
        private static ILogger _log;

        private static IContainer _container;

        protected Program()
        {
        }

        private static async Task Main()
        {
            try
            {
                Startup();
                await RefundPayments("input.txt");

                _log.LogInformation("Finished processing refunds");
                Environment.Exit(0);
            }
            catch (System.Exception ex)
            {
                _log.LogError("Error while processing refunds. Error={ex}", ex);
                Environment.Exit(1);
            }
        }

        private static void Startup()
        {
            // Load Configuration File
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new ContainerBuilder();

            // Load MobilePaySettings from Configuration
            builder.Register(c => configuration.GetSection("MobilePaySettings").Get<MobilePaySettings>())
                .SingleInstance();

            // setup DI for MobilePay services
            builder.RegisterType<CompletedOrderInputReader>().As<IInputReader<CompletedOrder>>();
            builder.RegisterType<MobilePayRefundOutputWriter>().As<IOutputWriter<IList<RefundResponse>>>();
            builder.RegisterType<MobilePayService>().As<IMobilePayService>();
            builder.RegisterType<RefundHandler>();
            builder.RegisterType<HttpClient>().SingleInstance();
            builder.RegisterType<MobilePayApiHttpClient>().As<IMobilePayApiHttpClient>().SingleInstance();
            builder.Register(c => new PhysicalFileProvider(Directory.GetCurrentDirectory())).As<IFileProvider>();

            builder.Register(l => LoggerFactory.Create(c => c.AddConsole())).As<ILoggerFactory>().SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            _container = builder.Build();

            _log = _container.Resolve<ILogger<Program>>();

            _log.LogInformation("Dependency Injection and configuration setup");
        }

        private static async Task RefundPayments(string path)
        {
            var mpInputReader = _container.Resolve<IInputReader<CompletedOrder>>();
            var completedOrders = await mpInputReader.ReadFromCommaSeparatedFile(path);

            var refundHandler = _container.Resolve<RefundHandler>();
            await refundHandler.RefundPayments(completedOrders);
        }
    }
}