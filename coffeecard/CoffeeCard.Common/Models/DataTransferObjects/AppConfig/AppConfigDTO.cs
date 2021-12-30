using CoffeeCard.Common.Configuration;

namespace CoffeeCard.Common.Models.DataTransferObjects.AppConfig
{
    public class AppConfigDto
    {
        public EnvironmentType EnvironmentType { get; set; }
        public string MerchantId { get; set; }
    }
}