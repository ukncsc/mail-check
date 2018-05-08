using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public class ReportFormatExplainer : BaseTagExplainerStrategy<ReportFormat>
    {
        public override string GetExplanation(ReportFormat tConcrete)
        {
            return DmarcExplainerResource.ReportFormatAFRFExplanation;
        }
    }
}