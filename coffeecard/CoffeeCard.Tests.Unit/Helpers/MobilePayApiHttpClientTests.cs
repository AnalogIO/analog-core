using System.Linq;
using System.Net.Http;
using CoffeeCard.Configuration;
using CoffeeCard.Helpers.MobilePay;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
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
            fileProvider.Setup(fp => fp.GetDirectoryContents(string.Empty)).Returns(directoryContents.Object);

            var environment = new Mock<IWebHostEnvironment>();
            environment.Setup(e => e.ContentRootFileProvider).Returns(fileProvider.Object);

            var httpClient = new Mock<HttpClient>();

            var mobileApiHttpClient =
                new MobilePayApiHttpClient(httpClient.Object, mobilePaySettings.Object, environment.Object);
        }
    }
}
