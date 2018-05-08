using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Contracts;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Utils.Conversion;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Linq;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;

namespace Dmarc.AggregateReport.Parser.Lambda
{
    public interface IPublishingEmailMessageInfoProcessor<TDomain> : IEmailMessageInfoProcessor<TDomain> where TDomain : class
    { }

    public class AggregateReportPublishingEmailMessageInfoProcessor<TDomain> : IPublishingEmailMessageInfoProcessor<TDomain>
        where TDomain : AggregateReportInfo
    {
        private const int IpBatchSize = 1000;

        private readonly IEmailMessageInfoProcessor<TDomain> _processor;
        private readonly IPublisher _publisher;
        private readonly ILogger _log;

        public AggregateReportPublishingEmailMessageInfoProcessor(IEmailMessageInfoProcessor<TDomain> processor,
            IPublisher publisher, ILogger log)
        {
            _processor = processor;
            _publisher = publisher;
            _log = log;
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
                        List<AggregateReportIpAddresses> messages = Create(result.Report);
                        _log.Info($"Publishing {messages.Count} aggregate report events, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}");
                        foreach (AggregateReportIpAddresses message in messages)
                        {
                            await _publisher.Publish(message);
                        }
                        _log.Info($"Succesfully published {messages.Count} aggregate report events, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}");
                    }
                    else
                    {
                        _log.Info($"Didnt publish aggregate report events for duplicate message, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId}");
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error($"Failed to publish aggregate report events, message Id: {messageInfo.EmailMetadata.MessageId}, request Id: {messageInfo.EmailMetadata.RequestId} with error {e.Message}{Environment.NewLine}{e.StackTrace}");
                return Result<TDomain>.FailedResult;
            }

            return result;
        }

        private List<AggregateReportIpAddresses> Create(AggregateReportInfo aggregateReportInfo)
        {
            DateTime effectiveDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReportInfo.AggregateReport.ReportMetadata.Range.EffectiveDate);
            List<string> ipAddresses = aggregateReportInfo.AggregateReport.Records.Select(_ => _.Row.SourceIp).Distinct().ToList();

            return ipAddresses
                .Batch(IpBatchSize)
                .Select(_ => new AggregateReportIpAddresses(aggregateReportInfo.EmailMetadata.RequestId, aggregateReportInfo.EmailMetadata.MessageId, effectiveDate, _.ToList()))
                .ToList();
        }
    }
}