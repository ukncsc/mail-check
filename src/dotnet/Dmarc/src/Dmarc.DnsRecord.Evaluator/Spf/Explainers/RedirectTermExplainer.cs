using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class RedirectTermExplainer : BaseTermExplainerStrategy<Redirect>
    {
        public override string GetExplanation(Redirect tConcrete)
        {
            return $"SPF record for {tConcrete.DomainSpec.Domain} replace the SPF record for this domain.";
        }
    }
}