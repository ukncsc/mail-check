using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.Report.Evnts;
using Dmarc.Common.Report.Logger;
using Dmarc.Common.Report.QueueProcessing;
using Dmarc.ForensicReport.Parser.Lambda.Factory;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Dmarc.ForensicReport.Parser.Lambda
{
    public class ForensicReportProcessor
    {
        private readonly IQueueProcessor _queueProcessor;
        private readonly LambdaLoggerAdaptor _log;

        public ForensicReportProcessor()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _log = new LambdaLoggerAdaptor();

            _log.Level = LogLevel.Trace;

            _queueProcessor = ForensicReportParserLambdaFactory.Create(_log);
            _log.Debug($"Creating parser took: {stopwatch.Elapsed}");
            stopwatch.Stop();
        }

        public async Task HandleScheduledEvent(ScheduledEvent evnt, ILambdaContext context)
        {
            try
            {
                await _queueProcessor.ProcessQueue(context).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _log.Error($"Failed to process forensic reports with following error {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }
    }
}
