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
        public static void AddConfigurationSettings(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.UseConfigurationValidation();

            // Parse and setup settings from configuration
            services.ConfigureValidatableSetting<DatabaseSettings>(
                configuration.GetSection("DatabaseSettings")
            );
            services.ConfigureValidatableSetting<EnvironmentSettings>(
                configuration.GetSection("EnvironmentSettings")
            );
            services.ConfigureValidatableSetting<LoginLimiterSettings>(
                configuration.GetSection("LoginLimiterSettings")
            );
            services.ConfigureValidatableSetting<IdentitySettings>(
                configuration.GetSection("IdentitySettings")
            );
            services.ConfigureValidatableSetting<MailgunSettings>(
                configuration.GetSection("MailgunSettings")
            );
            services.ConfigureValidatableSetting<MobilePaySettings>(
                configuration.GetSection("MobilePaySettings")
            );
            services.ConfigureValidatableSetting<OtlpSettings>(
                configuration.GetSection("OtlpSettings")
            );
        }
    }
}
