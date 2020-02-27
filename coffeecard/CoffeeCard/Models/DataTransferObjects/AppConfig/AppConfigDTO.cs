using CoffeeCard.Common.Configuration;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.AppConfig
{
    public class AppConfigDTO
    {
        public EnvironmentType EnvironmentType { get; set; }
        public string MerchantId { get; set; }
    }
}