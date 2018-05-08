using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.MxSecurityTester.Factory
{
    internal class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger()
            : base(System.Console.Error.WriteLine, LogLevel.Debug)
        {
        }
    }
}