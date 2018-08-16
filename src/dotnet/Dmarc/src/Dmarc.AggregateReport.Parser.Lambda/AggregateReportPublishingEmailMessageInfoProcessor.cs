using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Publishers;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;

namespace Dmarc.AggregateReport.Parser.Lambda
{
    public interface IPublishingEmailMessageInfoProcessor<TDomain> : IEmailMessageInfoProcessor<TDomain> where TDomain : class
    { }

    public class AggregateReportPublishingEmailMessageInfoProcessor<TDomain> : IPublishingEmailMessageInfoProcessor<TDomain>
        where TDomain : AggregateReportInfo
    {
        private readonly IEmailMessageInfoProcessor<TDomain> _processor;
        private readonly ILogger _log;
        private readonly List<IMessagePublisher> _publishers;

        public AggregateReportPublishingEmailMessageInfoProcessor(IEmailMessageInfoProcessor<TDomain> processor,
            IEnumerable<IMessagePublisher> publishers, ILogger log)
        {
            _processor = processor;
            _log = log;
            _publishers = publishers.ToList();
        }

        public async Task<Result<TDomain>> ProcessEmailMessage(EmailMessageInfo messageInfo)
        {
            Result<TDomain> result = await _processor.ProcessEmailMessage(messageInfo);

            try
            {
                if (result.Success)
                {
                    if (!result.Duplicate)
                    {
                        foreach (var publisher in _publishers)
                        {
                            await publisher.Publish(result.Report);
                        }
                    }
                    else
                    {
                        _log.Info($"Didnt publish aggregate aggregateReportInfo events for duplicate message, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}");
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Failed to publish aggregate aggregateReportInfo events, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId} with error {e.Message}{Environment.NewLine}{e.StackTrace}");
                return Result<TDomain>.FailedResult;
            }

            return result;
        }
    }
}