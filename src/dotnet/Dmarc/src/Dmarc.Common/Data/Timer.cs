using System;
using System.Diagnostics;

namespace Dmarc.Common.Data
{
    public class Timer : IDisposable
    {
        private readonly Action<string> _log;
        private readonly string _callName;
        private readonly Stopwatch _stopwatch;

        public Timer(Action<string> log, string callName)
        {
            _log = log;
            _callName = callName;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _log($"Call {_callName} took {_stopwatch.Elapsed}");
        }
    }
}