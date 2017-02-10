using System;
using Amazon.Lambda.Core;
using Dmarc.Common.Logging;

namespace Dmarc.AggregateReport.Api.Logger
{
    internal class LambdaLoggerAdaptor : AbstractLogger
    {
        public LambdaLoggerAdaptor() : base(s => LambdaLogger.Log($"{s}{Environment.NewLine}")) { }
    }
}
