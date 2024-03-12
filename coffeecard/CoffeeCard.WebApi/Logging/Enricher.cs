using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Linq;
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
    public class Enricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private const string UserIdPropertyName = "UserId";
        private static readonly string CorrelationIdItemName = $"{typeof(Enricher).Name}+CorrelationId";
        private readonly IHttpContextAccessor _contextAccessor;

        public Enricher() : this(new HttpContextAccessor())
        {
        }
        public Enricher(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext == null)
                return;

            LogEventProperty correlationIdProperty =
                new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(GetCorrelationId()));
            LogEventProperty userIdProperty = new LogEventProperty(UserIdPropertyName, new ScalarValue(GetUserId()));

            logEvent.AddOrUpdateProperty(correlationIdProperty);
            logEvent.AddOrUpdateProperty(userIdProperty);
        }

        private string GetUserId()
        {
            string id = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == Constants.UserId)
                ?.Value;

            return id != null ? $" userid:{id}" : string.Empty;
        }

        private string GetCorrelationId()
        {
            string id = $"correlationid:{Guid.NewGuid().ToString().Substring(0, 8)}";

            return (string)(_contextAccessor.HttpContext.Items[CorrelationIdItemName] ??
                             (_contextAccessor.HttpContext.Items[CorrelationIdItemName] = id));
        }
    }
}