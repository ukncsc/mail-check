using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.Common.Logging;

namespace Dmarc.AggregateReport.Parser.Lambda.Email
{
    internal interface IS3EmailMessageProcessor
    {
        Task ProcessEmailMessage(ILambdaContext context, S3Event s3Event);
    }

    internal class S3EmailMessageProcessor : IS3EmailMessageProcessor
    {
        private readonly ILambdaAggregateReportParserConfig _config;
        private readonly IS3EmailMessageClient _s3EmailMessageClient;
        private readonly IAggregateReportParserAsync _aggregateReportParser;
        private readonly ILogger _log;

        public S3EmailMessageProcessor(ILambdaAggregateReportParserConfig config,
            IS3EmailMessageClient s3EmailMessageClient,
            IAggregateReportParserAsync aggregateReportParser,
            ILogger log)
        {
            _config = config;
            _s3EmailMessageClient = s3EmailMessageClient;
            _aggregateReportParser = aggregateReportParser;
            _log = log;
        }

        public async Task ProcessEmailMessage(ILambdaContext context, S3Event s3Event)
        {
            List<EmailMessageInfo> emailMessageInfos = (await _s3EmailMessageClient.GetEmailMessages(context.AwsRequestId, s3Event)).ToList();

           
            List<EmailMessageInfo> emailMessageInfosOverThreshold = emailMessageInfos.Where(_ => _.EmailMetadata.FileSizeKb > _config.MaxS3ObjectSizeKilobytes).ToList();

            emailMessageInfosOverThreshold.ForEach(_ => _log.Warn($"Didn't process message as it's size ({_.EmailMetadata.FileSizeKb} Kb) exceeded max email message size {_config.MaxS3ObjectSizeKilobytes} Kb"));

            List<EmailMessageInfo> emailMessages = emailMessageInfos.Where(_ => _.EmailMetadata.FileSizeKb <= _config.MaxS3ObjectSizeKilobytes).ToList();

            await Task.WhenAll(emailMessages.Select(_aggregateReportParser.Parse));
        }
    }
}