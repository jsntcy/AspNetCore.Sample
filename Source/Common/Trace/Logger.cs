using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace AspNetCore.Sample.Common.Trace
{
    public static class Logger
    {
        private static readonly TelemetryClient _trace = new TelemetryClient();

        public static void TraceCritical(string error, IDictionary<string, string> properties = null)
        {
            _trace.TrackTrace(error, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Critical, properties);
        }

        public static void TraceError(string error, IDictionary<string, string> properties = null)
        {
            _trace.TrackTrace(error, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error, properties);
        }

        public static void TraceException(Exception ex, IDictionary<string, string> properties = null)
        {
            _trace.TrackException(ex, properties);
        }

        public static void TraceInfo(string info, IDictionary<string, string> properties = null)
        {
            _trace.TrackTrace(info, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information, properties);
        }

        public static void TraceWarning(string warning, IDictionary<string, string> properties = null)
        {
            _trace.TrackTrace(warning, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning, properties);
        }
    }
}
