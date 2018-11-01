using System;
using System.Diagnostics;
using Microsoft.ApplicationInsights;

namespace AspNetCore.Sample.Common.Trace
{
    public class DependencyScope : IDisposable
    {
        private static readonly TelemetryClient _telemetry = new TelemetryClient();
        private readonly DateTime _startTime;
        private readonly Stopwatch _timer;
        private readonly string _dependencyName;
        private readonly string _commandName;

        public DependencyScope(string dependencyName, string commandName)
        {
            _startTime = DateTime.UtcNow;
            _timer = Stopwatch.StartNew();
            _dependencyName = dependencyName;
            _commandName = commandName;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _timer.Stop();
            _telemetry.TrackDependency(_dependencyName, _dependencyName, _commandName, _startTime, _timer.Elapsed, true);
        }
    }
}
