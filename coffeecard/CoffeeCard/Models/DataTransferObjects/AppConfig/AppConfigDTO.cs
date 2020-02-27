using CoffeeCard.Common.Configuration;

namespace CoffeeCard.Models.DataTransferObjects.AppConfig
{
    public class AppConfigDTO
    {
        public EnvironmentType EnvironmentType { get; set; }
        public string MerchantId { get; set; }
    }
}