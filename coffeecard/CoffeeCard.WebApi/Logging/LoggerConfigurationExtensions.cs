using System;
using Serilog;
using Serilog.Configuration;

namespace CoffeeCard.WebApi.Logging
{
    /// <summary>
    /// Provides extension methods for configuring Serilog logging in the CoffeeCard.WebApi project.
    /// </summary>
    public static class LoggerConfigurationExtensions
    {
        /**
         * Add CorrelationId and UserId to log messages
         */
        public static LoggerConfiguration WithEnrichers(
            this LoggerEnrichmentConfiguration enrichmentConfiguration
        )
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<Enricher>();
        }
    }
}
