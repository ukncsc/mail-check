using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.AggregateReport.Parser.Lambda.Evnts;
using Dmarc.AggregateReport.Parser.Lambda.Factory;
using Dmarc.AggregateReport.Parser.Lambda.Logging;
using Dmarc.AggregateReport.Parser.Lambda.QueueProcessing;
using Dmarc.Common.Logging;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Dmarc.AggregateReport.Parser.Lambda
{
    public class AggregateReportProcessor
    {
        private readonly IQueueProcessor _queueProcessor;

        public AggregateReportProcessor()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ILogger log = new LambdaLoggerAdaptor();
            _queueProcessor = AggregateReportParserLambdaFactory.Create(log);
            log.Debug($"Creating parser took: {stopwatch.Elapsed}");
            stopwatch.Stop();
        }

        public async Task HandleScheduledEvent(ScheduledEvent evnt, ILambdaContext context)
        {
            await _queueProcessor.ProcessQueue(context).ConfigureAwait(false);
        }
    }
}
