using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AspNetCore.Sample.API.Filters
{
    public class RequestBodyTelemetryFilter : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        // link processors to each other in a chain.
        public RequestBodyTelemetryFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            // only trace request body for 4xx and 5xx
            if (item is RequestTelemetry requestTelemetry
                && int.TryParse(requestTelemetry.ResponseCode, out var code)
                && (code < 400 || code > 600))
            {
                if (requestTelemetry.Properties.TryGetValue("RequestBody", out _))
                {
                    requestTelemetry.Properties.Remove("RequestBody");
                }
            }

            _next.Process(item);
        }
    }
}
