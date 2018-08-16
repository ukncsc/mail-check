using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Contracts;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.AggregateReport.Parser.Lambda.Publishers
{
    public class DkimSelectorSeenPublisher : MessagePublisher
    {
        public DkimSelectorSeenPublisher(IPublisher publisher, ILogger log, IDkimSelectorPublisherConfig config)
            : base(publisher, log, config.PublisherConnectionString)
        {
        }

        public override List<object> Create(AggregateReportInfo aggregateReportInfo)
        {
            return aggregateReportInfo.AggregateReport.Records.SelectMany(_ => _.AuthResults.Dkim.Where(x => !string.IsNullOrEmpty(x.Selector)))
                .GroupBy(_ => _.Domain, _ => _.Selector)
                .Select(_ => new DkimSelectorsSeen(aggregateReportInfo.EmailMetadata.RequestId, aggregateReportInfo.EmailMetadata.MessageId, _.Key, _.Distinct().ToList()))
                .Cast<object>()
                .ToList();
        }
    }
}