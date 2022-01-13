using System;
using System.IO;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace CoffeeCard.MobilePay.GenerateApi
{
    /// <summary>
    /// Generate MobilePay Api client from OpenApi specification files in the 'OpenApiSpecs' directory
    /// and stores the C# files in 'CoffeeCard.MobilePay\Generated\'
    /// </summary>
    public static class Program
    {
        private const string PaymentsApi = "PaymentsApi";
        private const string WebhooksApi = "WebhooksApi";
        
        /// <summary>
        /// Generate MobilePay Api Payments and Webhooks client
        /// </summary>
        /// <exception cref="FileNotFoundException">OpenApi specification file could not be found</exception>
        public static async Task Main(string[] args)
        {
            var openApiSpecDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\OpenApiSpecs\\";
            var outputDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName +
                                  "\\CoffeeCard.MobilePay\\Generated\\";

            await GeneratePaymentsApi(openApiSpecDirectory + PaymentsApi + ".tojson.json", outputDirectory + $"{PaymentsApi}\\" + PaymentsApi + ".cs");
            await GenerateWebhooksApi(openApiSpecDirectory + WebhooksApi + ".tojson.json", outputDirectory + $"{WebhooksApi}\\" + WebhooksApi + ".cs");
        }

        private static async Task GenerateWebhooksApi(string inputFile, string outputFile)
        {
            CheckFileExists(inputFile);
            
            var document = await OpenApiDocument.FromFileAsync(inputFile);

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = WebhooksApi,
                CSharpGeneratorSettings =
                {
                    Namespace = $"CoffeeCard.MobilePay.Generated.Api.{WebhooksApi}"
                }
            };

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            await File.WriteAllTextAsync(outputFile, code);
        }

        private static async Task GeneratePaymentsApi(string inputFile, string outputFile)
        {
            CheckFileExists(inputFile);
            
            var document = await OpenApiDocument.FromFileAsync(inputFile);

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = PaymentsApi,
                CSharpGeneratorSettings =
                {
                    Namespace = $"CoffeeCard.MobilePay.Generated.Api.{PaymentsApi}"
                }
            };

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            await File.WriteAllTextAsync(outputFile, code);
        }

        private static void CheckFileExists(string inputFile)
        {
            var file = new FileInfo(inputFile);
            if (!file.Exists)
            {
                throw new FileNotFoundException(PaymentsApi);
            }
        }
    }
}