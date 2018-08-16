using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Contracts;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Utils.Conversion;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.Lambda.Publishers
{
    public class AggregateReportIpAddressesMessagePublisher : MessagePublisher
    {
        private const int IpBatchSize = 40;

        public AggregateReportIpAddressesMessagePublisher(IPublisher publisher, ILogger log, IPublisherConfig config)
            : base(publisher, log, config.PublisherConnectionString)
        {
        }

        public override List<object> Create(AggregateReportInfo aggregateReportInfo)
        {
            DateTime effectiveDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReportInfo.AggregateReport.ReportMetadata.Range.EffectiveDate);
            List<string> ipAddresses = aggregateReportInfo.AggregateReport.Records.Select(_ => _.Row.SourceIp).Distinct().ToList();

            return ipAddresses
                .Batch(IpBatchSize)
                .Select(_ => new AggregateReportIpAddresses(aggregateReportInfo.EmailMetadata.RequestId, aggregateReportInfo.EmailMetadata.MessageId, effectiveDate, _.ToList()))
                .Cast<object>()
                .ToList();
        }
    }
}