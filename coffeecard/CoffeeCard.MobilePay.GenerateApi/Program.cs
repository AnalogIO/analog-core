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
    ///
    /// MobilePay OpenApi specs are downloaded as yaml files from MobilePay Github repo https://github.com/MobilePayDev/MobilePayDev.github.io
    /// and then converted to JSON files as this is the file format NSwag is expecting
    /// </summary>
    public static class Program
    {
        private const string ePaymentApi = "ePaymentApi";
        private const string WebhooksApi = "WebhooksApi";
        private const string AccessToken = "AccessTokenApi";

        /// <summary>
        /// Generate MobilePay Api Payments and Webhooks client
        /// </summary>
        /// <exception cref="FileNotFoundException">OpenApi specification file could not be found</exception>
        public static async Task Main(string[] args)
        {
            var openApiSpecDirectory = Path.Combine(Environment.CurrentDirectory, "OpenApiSpecs");
            var outputDirectory = Path.Combine(
                Directory.GetParent(Environment.CurrentDirectory)!.FullName,
                "CoffeeCard.MobilePay",
                "Generated"
            );

            var paymentApiInput = Path.Combine(openApiSpecDirectory, ePaymentApi + ".tojson.json");
            var webhooksApiInput = Path.Combine(openApiSpecDirectory, WebhooksApi + ".tojson.json");
            var accessTokenApiInput = Path.Combine(
                openApiSpecDirectory,
                AccessToken + ".tojson.json"
            );

            var paymentApiOutput = Path.Combine(outputDirectory, ePaymentApi + ".cs");
            var webhooksApiOutput = Path.Combine(outputDirectory, WebhooksApi + ".cs");
            var accessTokenApiOutput = Path.Combine(outputDirectory, AccessToken + ".cs");

            await GenerateEPaymentApi(paymentApiInput, paymentApiOutput);
            await GenerateWebhooksApi(webhooksApiInput, webhooksApiOutput);
            await GenerateAccessTokenApi(accessTokenApiInput, accessTokenApiOutput);
        }

        private static async Task GenerateWebhooksApi(string inputFile, string outputFile)
        {
            CheckFileExists(inputFile);

            var document = await OpenApiDocument.FromFileAsync(inputFile);

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = WebhooksApi,
                GenerateClientClasses = false,
                CSharpGeneratorSettings =
                {
                    Namespace = $"CoffeeCard.MobilePay.Generated.Api.{WebhooksApi}",
                },
                UseBaseUrl = false,
            };

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            await File.WriteAllTextAsync(outputFile, code);
        }

        private static async Task GenerateEPaymentApi(string inputFile, string outputFile)
        {
            CheckFileExists(inputFile);

            var document = await OpenApiDocument.FromFileAsync(inputFile);

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = ePaymentApi,
                GenerateClientClasses = false,
                CSharpGeneratorSettings =
                {
                    Namespace = $"CoffeeCard.MobilePay.Generated.Api.{ePaymentApi}",
                },
                UseBaseUrl = false,
            };

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            await File.WriteAllTextAsync(outputFile, code);
        }

        private static async Task GenerateAccessTokenApi(string inputFile, string outputFile)
        {
            CheckFileExists(inputFile);

            var document = await OpenApiDocument.FromFileAsync(inputFile);

            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = AccessToken,
                GenerateClientClasses = false,
                CSharpGeneratorSettings =
                {
                    Namespace = $"CoffeeCard.MobilePay.Generated.Api.{AccessToken}",
                },
                UseBaseUrl = false,
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
                throw new FileNotFoundException(inputFile);
            }
        }
    }
}
