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
using CoffeeCard.MobilePay.Service.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.RefundConsoleApp
{
    internal class Program
    {
        private ILogger _log;

        private IContainer _container;

        protected Program(IContainer container)
        {
            _container = container;
            _log = _container.Resolve<ILogger<Program>>();
            _log.LogInformation("Dependency Injection and configuration setup");
        }

        private static async Task Main()
        {
            var program = Startup();
            try
            {
                await program.RefundPayments("input.txt");

                program._log.LogInformation("Finished processing refunds");
                Environment.Exit(0);
            }
            catch (System.Exception ex)
            {
                program._log.LogError("Error while processing refunds. Error={ex}", ex);
                Environment.Exit(1);
            }
        }

        private static Program Startup()
        {
            // Load Configuration File
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var builder = new ContainerBuilder();

            // Load MobilePaySettings from Configuration
            builder
                .Register(c =>
                    configuration.GetSection("MobilePaySettings").Get<MobilePaySettings>()
                )
                .SingleInstance();

            // setup DI for MobilePay services
            builder.RegisterType<CompletedOrderInputReader>().As<IInputReader<CompletedOrder>>();
            builder
                .RegisterType<MobilePayRefundOutputWriter>()
                .As<IOutputWriter<IList<RefundResponse>>>();
            builder.RegisterType<MobilePayService>().As<IMobilePayService>();
            builder.RegisterType<RefundHandler>();
            builder.RegisterType<HttpClient>().SingleInstance();
            builder
                .RegisterType<MobilePayApiHttpClient>()
                .As<IMobilePayApiHttpClient>()
                .SingleInstance();
            builder
                .Register(c => new PhysicalFileProvider(Directory.GetCurrentDirectory()))
                .As<IFileProvider>();

            builder
                .Register(l => LoggerFactory.Create(c => c.AddConsole()))
                .As<ILoggerFactory>()
                .SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();

            return new Program(builder.Build());
        }

        private async Task RefundPayments(string path)
        {
            var mpInputReader = _container.Resolve<IInputReader<CompletedOrder>>();
            var completedOrders = await mpInputReader.ReadFromCommaSeparatedFile(path);

            var refundHandler = _container.Resolve<RefundHandler>();
            await refundHandler.RefundPayments(completedOrders);
        }
    }
}
