using System;
using CoffeeCard.WebApi.Logging.Enrichers;
using Serilog;
using Serilog.Configuration;
using Serilog.Enrichers;

namespace CoffeeCard.WebApi.Logging
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<CorrelationIdEnricher>();
        }
        
        public static LoggerConfiguration WithUserId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<UserIdEnricher>();
        }
    }
}