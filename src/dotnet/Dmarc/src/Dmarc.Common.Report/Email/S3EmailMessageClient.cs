using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Config;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Tasks;

namespace Dmarc.Common.Report.Email
{
    public interface IS3EmailMessageClient
    {
        Task<EmailMessageInfo[]> GetEmailMessages(string requestId, string messageId, S3Event evnt);
    }

    public class S3EmailMessageClient : IS3EmailMessageClient
    {
        private readonly ILambdaReportParserConfig _config;
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger _log;

        public S3EmailMessageClient(ILambdaReportParserConfig config, IAmazonS3 s3Client, ILogger log)
        {
            _config = config;
            _s3Client = s3Client;
            _log = log;
        }

        public Task<EmailMessageInfo[]> GetEmailMessages(string requestId, string messageId, S3Event evnt)
        {
            return Task.WhenAll(evnt.Records.Select(_ => CreateEmailMessageAsync(requestId, messageId, _)));
        }

        private async Task<EmailMessageInfo> CreateEmailMessageAsync(string requestId, string messageId, S3EventNotification.S3EventNotificationRecord record)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            GetObjectResponse response = await _s3Client.GetObjectAsync(record.S3.Bucket.Name, record.S3.Object.Key)
                .TimeoutAfter(_config.TimeoutS3)
                .ConfigureAwait(false);

            _log.Debug($"Retrieving aggregate report from bucket took {stopwatch.Elapsed}");

            stopwatch.Stop();

            string originalUri = $"{record.S3.Bucket.Name}/{record.S3.Object.Key}";
  
            return new EmailMessageInfo(new EmailMetadata(requestId, messageId, originalUri, record.S3.Object.Key, record.S3.Object.Size / 1024), response.ResponseStream);
        }
    }
}