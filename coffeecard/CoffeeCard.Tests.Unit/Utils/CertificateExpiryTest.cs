using System;
using Xunit;

namespace CoffeeCard.Tests.Unit.Utils
{
    public class CertificateExpiryTest
    {
        [Fact(DisplayName = "Test MobilePay API certificate is not expiring with the next three months")]
        public void TestMobilePayApiCertificateExpirtyDate()
        {
            TimeSpan threeMonths = TimeSpan.FromDays(90.0);
            DateTime today = DateTime.UtcNow.Date;

            // Read from actual date from certificate
            DateTime certificateExpiryDate = DateTime.Parse("2024-06-17");

            Assert.True(today.Add(threeMonths).CompareTo(certificateExpiryDate) == -1,
                $"MobilePay API Certificate is expiring within the next three months. Today's date = {today}, ExpiryDate = {certificateExpiryDate}");
        }
    }
}