using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Config;
using Dmarc.Common.Report.Domain;

namespace Dmarc.Common.Report.Email
{
    public interface IS3EmailMessageProcessor
    {
        Task<bool> ProcessEmailMessage(string messagId, ILambdaContext context, S3Event s3Event);
    }

    public class S3EmailMessageProcessor<TDomain>  : IS3EmailMessageProcessor
        where TDomain : class
    {
        private readonly IS3EmailMessageClient _s3EmailMessageClient;
        private readonly ILambdaReportParserConfig _config;
        private readonly ILogger _log;
        private readonly IEmailMessageInfoProcessor<TDomain> _emailMessageInfoProcessor;

        public S3EmailMessageProcessor(
            IS3EmailMessageClient s3EmailMessageClient,
            IEmailMessageInfoProcessor<TDomain> emailMessageInfoProcessor,
            ILambdaReportParserConfig config,
            ILogger log)
        {
            _s3EmailMessageClient = s3EmailMessageClient;
            _config = config;
            _log = log;
            _emailMessageInfoProcessor = emailMessageInfoProcessor;
        }

        public async Task<bool> ProcessEmailMessage(string messageId, ILambdaContext context, S3Event s3Event)
        {
            EmailMessageInfo[] emailMessageInfos  = await _s3EmailMessageClient.GetEmailMessages(context.AwsRequestId, messageId, s3Event);

            if (emailMessageInfos.Any(_ => _.EmailMetadata.FileSizeKb > _config.MaxS3ObjectSizeKilobytes))
            {
                _log.Warn($"Didn't process messages as MaxS3ObjectSizeKilobytes of {_config.MaxS3ObjectSizeKilobytes} Kb was exceeded");
                return false;
            }

            List<Result<TDomain>> results = new List<Result<TDomain>>();
            foreach (EmailMessageInfo emailMessageInfo in emailMessageInfos)
            {
                results.Add(await _emailMessageInfoProcessor.ProcessEmailMessage(emailMessageInfo));
            }

            return results.Any() && results.All(_ => _.Success);
        }
    }
}