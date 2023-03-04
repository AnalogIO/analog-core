using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.ErrorMessage;
using CoffeeCard.MobilePay.Exception;
using CoffeeCard.MobilePay.RequestMessage;
using CoffeeCard.MobilePay.ResponseMessage;
using Jose;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CoffeeCard.MobilePay.Client
{
    public class MobilePayApiHttpClient : IMobilePayApiHttpClient
    {
        private const string MobilePayBaseEndpoint = "https://api.mobeco.dk/appswitch/api/v1/";

        private readonly HttpClient _client;
        private readonly IFileProvider _fileProvider;
        private readonly ILogger<MobilePayApiHttpClient> _log;
        private readonly MobilePaySettings _mobilePaySettings;

        public MobilePayApiHttpClient(HttpClient client, MobilePaySettings mobilePaySettings,
            IFileProvider fileProvider, ILogger<MobilePayApiHttpClient> log)
        {
            _mobilePaySettings = mobilePaySettings;
            _client = client;
            _log = log;
            _fileProvider = fileProvider;
        }

        public async Task<T> SendRequest<T>(IMobilePayApiRequestMessage requestMessage) where T : IMobilePayApiResponse
        {
            var requestUri = new Uri(MobilePayBaseEndpoint + requestMessage.GetEndPointUri());

            var request = new HttpRequestMessage
            {
                Headers =
                {
                    {
                        "AuthenticationSignature",
                        GenerateAuthenticationSignature(requestUri.ToString(), requestMessage.GetRequestBody())
                    },
                    {"Ocp-Apim-Subscription-Key", _mobilePaySettings.SubscriptionKey}
                },
                Method = requestMessage.GetHttpMethod(),
                RequestUri = requestUri,
                Content = new StringContent(requestMessage.GetRequestBody(), Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode) await HandleHttpErrorAsync(response);

            return await response.Content.ReadAsAsync<T>();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private async Task HandleHttpErrorAsync(HttpResponseMessage response)
        {
            _log.LogError(
                $"HTTP Response failed with statusCode = {response.StatusCode} and message = {response.Content.ReadAsStringAsync().Result}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                {
                    var errorMessage = await response.Content.ReadAsAsync<InternalServerErrorMessage>();
                    throw new MobilePayException(errorMessage, HttpStatusCode.InternalServerError);
                }
                case HttpStatusCode.RequestTimeout:
                {
                    throw new MobilePayException(new DefaultErrorMessage
                    {
                        Reason = MobilePayErrorReason.Other
                    }, HttpStatusCode.RequestTimeout);
                }
                default:
                {
                    var errorMessage = await response.Content.ReadAsAsync<DefaultErrorMessage>();
                    throw new MobilePayException(errorMessage, response.StatusCode);
                }
            }
        }

        private X509Certificate2? LoadCertificate(IFileProvider fileProvider)
        {
            var certName = _mobilePaySettings.CertificateName;

            var contents = fileProvider.GetDirectoryContents(string.Empty);
            var certPath = contents.FirstOrDefault(file => file.Name.Equals(certName))?.PhysicalPath;
            
            if (certPath == null) 
            {
                return null;
            }

            return new X509Certificate2(certPath, _mobilePaySettings.CertificatePassword,
                X509KeyStorageFlags.MachineKeySet);
        }

        private string GenerateAuthenticationSignature(string requestUri, string requestBody)
        {
            var combinedRequest = requestUri + requestBody;

#pragma warning disable CA5350 // SHA1 is used per Mobile Pay documentation but considered weak. Ignore compiler warning
            var sha1 = SHA1.Create();
#pragma warning restore CA5350
            var hash = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(combinedRequest)));
            var loadedCert = LoadCertificate(_fileProvider);
            ArgumentNullException.ThrowIfNull(loadedCert);

            using var rsa = loadedCert.GetRSAPrivateKey();
            
            var signature = JWT.Encode(hash, rsa, JwsAlgorithm.RS256);
            return signature;
        }
    }
}