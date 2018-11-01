using System;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace AspNetCore.Sample.API.Filters
{
    public class RequestBodyTelemetryInitializer : ITelemetryInitializer
    {
        private const string RequestBody = "RequestBody";
        private const string Referer = "Referer";
        private const string RemoteIpAddress = "RemoteIpAddress";
        private IHttpContextAccessor _httpContextAccessor;

        public RequestBodyTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Initialize(ITelemetry telemetry)
        {
            var requestTelemetry = telemetry as RequestTelemetry;

            if (requestTelemetry == null)
            {
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            requestTelemetry.Properties[RemoteIpAddress] = $"{httpContext.Connection.RemoteIpAddress}";
            if (httpContext.Request.Headers.ContainsKey(Referer))
            {
                requestTelemetry.Properties[Referer] = httpContext.Request.Headers[Referer];
            }

            if (int.TryParse(requestTelemetry.ResponseCode, out _))
            {
                return;
            }

            if (httpContext.Request == null)
            {
                return;
            }

            if ((httpContext.Request.Method == HttpMethods.Put || httpContext.Request.Method == HttpMethods.Post) && httpContext.Request.Body != null)
            {
                if (!requestTelemetry.Properties.TryGetValue(RequestBody, out _))
                {
                    requestTelemetry.Properties[RequestBody] = FormatRequest(httpContext.Request);
                }
            }
        }

        private string FormatRequest(HttpRequest request)
        {
            try
            {
                request.EnableRewind();

                var buffer = new byte[Convert.ToInt32(request.ContentLength)];
                request.Body.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
            catch (Exception)
            {
                // do nothing
            }
            finally
            {
                request.Body.Position = 0;
            }

            return string.Empty;
        }
    }
}
