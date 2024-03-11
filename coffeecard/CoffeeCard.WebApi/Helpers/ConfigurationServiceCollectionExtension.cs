using CoffeeCard.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCard.WebApi.Helpers
{
    /// <summary>
    /// Extenstion for setting up configuration classes
    /// </summary>
    public static class ConfigurationServiceCollectionExtension
    {
        /// <summary>
        /// Add and validate Configuration Settings classes from the configuration file
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">Reference to configuration file</param>
        public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.UseConfigurationValidation();

            // Parse and setup settings from configuration
            _ = services.ConfigureValidatableSetting<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            _ = services.ConfigureValidatableSetting<EnvironmentSettings>(configuration.GetSection("EnvironmentSettings"));
            _ = services.ConfigureValidatableSetting<LoginLimiterSettings>(configuration.GetSection("LoginLimiterSettings"));
            _ = services.ConfigureValidatableSetting<IdentitySettings>(configuration.GetSection("IdentitySettings"));
            _ = services.ConfigureValidatableSetting<MailgunSettings>(configuration.GetSection("MailgunSettings"));
            _ = services.ConfigureValidatableSetting<MobilePaySettingsV2>(configuration.GetSection("MobilePaySettingsV2"));
        }
    }
}