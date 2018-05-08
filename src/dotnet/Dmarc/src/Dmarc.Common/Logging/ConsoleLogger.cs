using System;
using Dmarc.Common.Interface.Logging;

namespace Dmarc.Common.Logging
{
    public class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger()
            : base(Console.Error.WriteLine, LogLevel.Debug)
        {
        }
    }
}