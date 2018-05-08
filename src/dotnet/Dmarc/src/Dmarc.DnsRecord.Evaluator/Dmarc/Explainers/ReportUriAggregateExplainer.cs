using System;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class ReportUriAggregateExplainer : BaseTagExplainerStrategy<ReportUriAggregate>
    {
        public override string GetExplanation(ReportUriAggregate tConcrete)
        {
            string uris = string.Join(Environment.NewLine, tConcrete.Uris.Select(_ => $"{_.Uri.Uri.UserInfo}@{_.Uri.Uri.Host}"));
            return string.Format(DmarcExplainerResource.ReportUriAggregateExplanation, uris);
        }
    }
}