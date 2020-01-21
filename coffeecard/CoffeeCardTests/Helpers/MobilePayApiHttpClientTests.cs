using System;
using System.Linq;
using System.Net.Http;
using CoffeeCard.Helpers.MobilePay;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace CoffeeCardTests.Helpers
{
	public class MobilePayApiHttpClientTests
	{
		[Fact(DisplayName = "")]
		public void test()
		{
			var testCertificateName = "testCertificate.pfx";

			var configuration = new Mock<IConfiguration>();
			configuration.Setup(c => c["CertificatePassword"]).Returns("password");
			configuration.Setup(c => c["MPSubscriptionKey"]).Returns("subscrpKey");
			configuration.Setup(c => c["MobilePayAPI-CertificateName"]).Returns(testCertificateName);

			var directoryContents = new Mock<IDirectoryContents>();
			directoryContents.Setup(dc => dc.FirstOrDefault(f => f.Name.Equals(testCertificateName)).PhysicalPath)
				.Returns("");

			var fileProvider = new Mock<IFileProvider>();
			fileProvider.Setup(fp => fp.GetDirectoryContents(string.Empty)).Returns(directoryContents.Object);

			var environment = new Mock<IHostingEnvironment>();
			environment.Setup(e => e.ContentRootFileProvider).Returns(fileProvider.Object);

			var httpClient = new Mock<HttpClient>();

			var mobileApiHttpClient = new MobilePayApiHttpClient(httpClient.Object, configuration.Object, environment.Object);
		}
	}
}
