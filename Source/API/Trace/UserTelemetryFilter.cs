using System.Linq;

using AspNetCore.Sample.API.Helpers;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Apex.Security.Constants;

namespace AspNetCore.Sample.API.Filters
{
    public class UserTelemetryFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        // link processors to each other in a chain.
        public UserTelemetryFilter(ITelemetryProcessor next, IHttpContextAccessor httpContextAccessor)
        {
            Next = next;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry requestTelemetry)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext.User?.Claims?.Count() > 0)
                {
                    var docsUserIdClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.DocsUserId);
                    if (docsUserIdClaim != null)
                    {
                        var userId = ClaimHelper.ExtractUserIdFromClaim(docsUserIdClaim.Value);
                        requestTelemetry.Context.User.Id = userId;
                        requestTelemetry.Context.User.AuthenticatedUserId = userId;
                    }
                }
            }

            Next.Process(item);
        }
    }
}
