using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.Common.Logging;
using Dmarc.Common.Tasks;

namespace Dmarc.AggregateReport.Parser.Lambda.Email
{
    internal interface IS3EmailMessageClient
    {
        Task<EmailMessageInfo[]> GetEmailMessages(string awsRequestId, S3Event evnt);
    }

    internal class S3EmailMessageClient : IS3EmailMessageClient
    {
        private readonly ILambdaAggregateReportParserConfig _config;
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger _log;

        public S3EmailMessageClient(ILambdaAggregateReportParserConfig config, IAmazonS3 s3Client, ILogger log)
        {
            _config = config;
            _s3Client = s3Client;
            _log = log;
        }

        public Task<EmailMessageInfo[]> GetEmailMessages(string awsRequestId, S3Event evnt)
        {
            return Task.WhenAll(evnt.Records.Select(_ => CreateEmailMessageAsync(awsRequestId, _)));
        }

        private async Task<EmailMessageInfo> CreateEmailMessageAsync(string awsRequestId, S3EventNotification.S3EventNotificationRecord record)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            GetObjectResponse response = await _s3Client.GetObjectAsync(record.S3.Bucket.Name, record.S3.Object.Key)
                .TimeoutAfter(_config.TimeoutS3)
                .ConfigureAwait(false);

            _log.Debug($"Retrieving aggregregate report from bucket took {stopwatch.Elapsed}");

            stopwatch.Stop();

            string originalUri = $"{record.S3.Bucket.Name}/{record.S3.Object.Key}";
  
            return new EmailMessageInfo(new EmailMetadata(awsRequestId, originalUri, record.S3.Object.Key, record.S3.Object.Size / 1024), response.ResponseStream);
        }
    }
}