using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using Constants = CoffeeCard.Common.Constants;

namespace CoffeeCard.WebApi.Logging
{
    // This code is a based on the Serilog.Enrichers.CorrelationId project
    // https://github.com/ekmsystems/serilog-enrichers-correlation-id
    //
    // MIT License
    //
    // Copyright (c) 2017 Steven Atkinson
    //
    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
    //
    // The above copyright notice and this permission notice shall be included in all
    // copies or substantial portions of the Software.
    //
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    // SOFTWARE.
    /// <summary>
    /// Enriches log events with correlation ID and user ID information from the current HTTP context.
    /// </summary>
    public class Enricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private const string UserIdPropertyName = "UserId";
        private static readonly string CorrelationIdItemName =
            $"{typeof(Enricher).Name}+CorrelationId";
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enricher"/> class.
        /// </summary>
        public Enricher()
            : this(new HttpContextAccessor()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enricher"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public Enricher(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Enriches the log event with correlation ID and user ID information from the current HTTP context.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">The property factory.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext == null)
                return;

            var correlationIdProperty = new LogEventProperty(
                CorrelationIdPropertyName,
                new ScalarValue(GetCorrelationId())
            );
            var userIdProperty = new LogEventProperty(
                UserIdPropertyName,
                new ScalarValue(GetUserId())
            );

            logEvent.AddOrUpdateProperty(correlationIdProperty);
            logEvent.AddOrUpdateProperty(userIdProperty);
        }

        private string GetUserId()
        {
            var id = _contextAccessor
                .HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)
                ?.Value;

            return id != null ? $" userid:{id}" : string.Empty;
        }

        private string GetCorrelationId()
        {
            var id = $"correlationid:{Guid.NewGuid().ToString().Substring(0, 8)}";

            return (string)(
                _contextAccessor.HttpContext.Items[CorrelationIdItemName]
                ?? (_contextAccessor.HttpContext.Items[CorrelationIdItemName] = id)
            );
        }
    }
}
