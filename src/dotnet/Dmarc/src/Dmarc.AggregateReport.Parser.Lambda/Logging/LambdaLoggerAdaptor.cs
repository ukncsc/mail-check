using System;
using Amazon.Lambda.Core;
using Dmarc.Common.Logging;

namespace Dmarc.AggregateReport.Parser.Lambda.Logging
{
    internal class LambdaLoggerAdaptor : AbstractLogger
    {
        public LambdaLoggerAdaptor() : base(s => LambdaLogger.Log($"{s}{Environment.NewLine}")){}
    }
}
