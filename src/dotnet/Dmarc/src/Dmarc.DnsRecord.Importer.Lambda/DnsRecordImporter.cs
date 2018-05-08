using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.Report.Evnts;
using Dmarc.Common.Report.Logger;
using Dmarc.DnsRecord.Importer.Lambda.RecordProcessor;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Dmarc.DnsRecord.Importer.Lambda
{
    public abstract class DnsRecordImporter
    {
        private readonly string _recordType;
        private readonly IDnsRecordProcessor _dnsRecordProcessor;
        private readonly ILogger _log;

        protected DnsRecordImporter(string recordType, Func<ILogger,IDnsRecordProcessor> recordProcessorFactory)
        {
            _recordType = recordType;
            Stopwatch stopwatch = Stopwatch.StartNew();
            _log = new LambdaLoggerAdaptor();

            _dnsRecordProcessor = recordProcessorFactory(_log);

            _log.Debug($"Creating {recordType}RecordImporter took: {stopwatch.Elapsed}");
            stopwatch.Stop();
        }

        public async Task HandleScheduledEvent(ScheduledEvent evnt, ILambdaContext context)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                await _dnsRecordProcessor.Process(context);
                _log.Debug($"Importing {_recordType} Records took: {stopwatch.Elapsed}");
                stopwatch.Stop();
            }
            catch (Exception e)
            {
                _log.Error($"Failed to import {_recordType} Records with error {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }
    }
}