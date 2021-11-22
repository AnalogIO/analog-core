using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace CoffeeCard.WebApi.Logging.Enrichers
{
    public class UserIdEnricher : ILogEventEnricher
    {
        private const string UserIdPropertyName = "UserId";
        private readonly IHttpContextAccessor _contextAccessor;

        public UserIdEnricher() : this(new HttpContextAccessor())
        {
        }

        private UserIdEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext == null)
                return;

            var userIdProperty = new LogEventProperty(UserIdPropertyName, new ScalarValue(GetUserId()));

            logEvent.AddOrUpdateProperty(userIdProperty);
        }

        private string GetUserId()
        {
            var id =  _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return $"userid:{id}";
        }
    }
}