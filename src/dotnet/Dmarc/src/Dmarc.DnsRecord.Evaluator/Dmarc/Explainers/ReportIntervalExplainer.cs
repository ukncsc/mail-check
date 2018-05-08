using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class ReportIntervalExplainer : BaseTagExplainerStrategy<ReportInterval>
    {
        public override string GetExplanation(ReportInterval tConcrete)
        {
            return string.Format(DmarcExplainerResource.ReportIntervalExplanation,
                tConcrete.Interval.Value, tConcrete.Interval.Value / 3600);
        }
    }
}