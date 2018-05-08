using System;
using System.Threading;
using Dmarc.Common.Interface.Logging;

namespace Dmarc.Common.Logging
{
    public abstract class AbstractLogger : ILogger
    {
        private readonly Action<string> _write;

        protected AbstractLogger(Action<string> write)
            : this(write, LogLevel.Debug)
        {
        }

        protected AbstractLogger(Action<string> write, LogLevel level)
        {
            Level = level;
            _write = write;
        }
        public void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        private void Log(LogLevel level, string message)
        {
            if (Level <= level)
            {
                _write($"[{level}] [{Thread.CurrentThread.ManagedThreadId}] : {message}");
            }
        }

        public LogLevel Level { get; set; }
    }
}