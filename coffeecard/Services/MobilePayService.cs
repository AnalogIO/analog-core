using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CoffeeCard.Services
{
    public class MobilePayService : IMobilePayService
    {
        private readonly X509Certificate2 _cert;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public MobilePayService(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            var provider = env.ContentRootFileProvider;
            var contents = provider.GetDirectoryContents(string.Empty);
            var certPath = contents.FirstOrDefault(x => x.Name.Equals("www.analogio.dk.pfx")).PhysicalPath;

            _cert = new X509Certificate2(certPath, _configuration["CertificatePassword"],
                X509KeyStorageFlags.MachineKeySet);

            _client = new HttpClient();
        }

        public async Task<HttpResponseMessage> CheckOrderIdAgainstMPBackendAsync(string orderId)
        {
            Log.Information($"Checking order against mobilepay with orderId: {orderId}");
            var payload =
                $"https://api.mobeco.dk/appswitch/api/v1/merchants/{_configuration["MPMerchantID"]}/orders/{orderId}";

            var response = await SendRequest(payload);
            return response;
        }

        private string GetSignatureForPayload(string payload)
        {
            var sha1 = new SHA1Managed();
            var hash = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            using (var rsa = _cert.GetRSAPrivateKey())
            {
                var signature = JWT.Encode(hash, rsa, JwsAlgorithm.RS256);
                return signature;
            }
        }

        private async Task<HttpResponseMessage> SendRequest(string payload)
        {
            var signature = GetSignatureForPayload(payload);

            var request = new HttpRequestMessage
            {
                Headers =
                {
                    {"AuthenticationSignature", signature},
                    {"Ocp-Apim-Subscription-Key", _configuration["MPSubscriptionKey"]}
                },
                Method = HttpMethod.Get,
                RequestUri = new Uri(payload)
            };

            var response = await _client.SendAsync(request);
            return response;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}