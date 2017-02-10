using System;
using Dmarc.Common.Logging;

namespace Dmarc.AggregateReport.Parser.App.Logging
{
    internal class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger() 
            : base(Console.Error.WriteLine, LogLevel.Debug)
        {
        }
    }
}
