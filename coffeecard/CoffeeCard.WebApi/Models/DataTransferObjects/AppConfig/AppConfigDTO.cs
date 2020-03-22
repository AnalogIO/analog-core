using CoffeeCard.Common.Configuration;

namespace CoffeeCard.WebApi.Models.DataTransferObjects.AppConfig
{
    public class AppConfigDto
    {
        public EnvironmentType EnvironmentType { get; set; }
        public string MerchantId { get; set; }
    }
}