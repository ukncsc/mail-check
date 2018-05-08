using Amazon.Lambda.Core;
using Dmarc.Common.Logging;

namespace Dmarc.Common.Report.Logger
{
    public class LambdaLoggerAdaptor : AbstractLogger
    {
        public LambdaLoggerAdaptor() : base(s => LambdaLogger.Log($"{s}{System.Environment.NewLine}")) { }
    }
}
