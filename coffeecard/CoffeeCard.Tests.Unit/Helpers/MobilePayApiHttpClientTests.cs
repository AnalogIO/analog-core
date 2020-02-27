using System;
using System.Linq;
using System.Net.Http;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Client;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeCard.Tests.Unit.Helpers
{
    public class MobilePayApiHttpClientTests
    {
        //[Fact(DisplayName = "")]
        public void test()
        {
            var testCertificateName = "testCertificate.pfx";

            var mobilePaySettings = new Mock<MobilePaySettings>();
            mobilePaySettings.Setup(c => c.CertificatePassword).Returns("password");
            mobilePaySettings.Setup(c => c.SubscriptionKey).Returns("subscrpKey");
            mobilePaySettings.Setup(c => c.CertificateName).Returns(testCertificateName);

            var directoryContents = new Mock<IDirectoryContents>();
            directoryContents.Setup(dc => dc.FirstOrDefault(f => f.Name.Equals(testCertificateName)).PhysicalPath)
                .Returns("");

            var fileProvider = new Mock<IFileProvider>();
            fileProvider.Setup(e => e.GetDirectoryContents(String.Empty)).Returns(directoryContents.Object);

            var httpClient = new Mock<HttpClient>();
            var logger = new Mock<ILogger<MobilePayApiHttpClient>>();

            var mobileApiHttpClient =
                new MobilePayApiHttpClient(httpClient.Object, mobilePaySettings.Object, fileProvider.Object, logger.Object);
        }
    }
}
