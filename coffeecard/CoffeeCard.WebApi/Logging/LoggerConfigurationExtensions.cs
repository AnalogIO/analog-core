using System;
using Serilog;
using Serilog.Configuration;

namespace CoffeeCard.WebApi.Logging
{
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
