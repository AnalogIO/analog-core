using Jose;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace coffeecard.Services
{
    public class MobilePayService : IMobilePayService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly X509Certificate2 _cert;

        public MobilePayService(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            var provider = env.ContentRootFileProvider;
            var contents = provider.GetDirectoryContents(string.Empty);
            var certPath = contents.FirstOrDefault(x => x.Name.Equals("test_MobilePay_AnalogIO.pfx")).PhysicalPath;

            _cert = new X509Certificate2(certPath, _configuration["CertificatePassword"], X509KeyStorageFlags.MachineKeySet);

            _client = new HttpClient();
        }

        private string GetSignatureForPayload(string payload)
        {
            var sha1 = new SHA1Managed();
            var hash = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            using (RSA rsa = _cert.GetRSAPrivateKey())
            {
                var signature = JWT.Encode(hash, rsa, JwsAlgorithm.RS256);
                return signature;
            }

        }

        public async Task<HttpResponseMessage> GetPaymentStatus(string orderId)
        {
            Log.Information($"Checking order against mobilepay with orderId: {orderId}");
            var merchantId = _configuration["MPMerchantID"];
            var payload = $"https://api.mobeco.dk/appswitch/api/v1/merchants/{merchantId}/orders/{orderId}";

            var response = await SendRequest(payload, HttpMethod.Get);
            return response;
        }

        public async Task<HttpResponseMessage> CaptureAmount(string orderId)
        {
            var merchantId = _configuration["MPMerchantID"];
            var payload = $"https://api.mobeco.dk/appswitch/api/v1/reservations/merchants/{merchantId}/orders/{orderId}";

            var response = await SendRequest(payload, HttpMethod.Put);
            return response;
        }

        public async Task<HttpResponseMessage> CancelPaymentReservation(string orderId)
        {
            var merchantId = _configuration["MPMerchantID"];
            var payload = $"https://api.mobeco.dk/appswitch/api/v1/reservations/merchants/{merchantId}/orders/{orderId}";

            var response = await SendRequest(payload, HttpMethod.Delete);
            return response;
        }

        private async Task<HttpResponseMessage> SendRequest(string payload, HttpMethod httpMethod)
        {
            var signature = GetSignatureForPayload(payload);

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Headers = { { "AuthenticationSignature", signature }, { "Ocp-Apim-Subscription-Key", _configuration["MPSubscriptionKey"] } },
                Method = httpMethod,
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