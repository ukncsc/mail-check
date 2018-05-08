using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class PtrTermExplainer : BaseTermExplainerStrategy<Ptr>
    {
        private readonly IQualifierExplainer _qualifierExplainer;

        public PtrTermExplainer(IQualifierExplainer qualifierExplainer)
        {
            _qualifierExplainer = qualifierExplainer;
        }

        public override string GetExplanation(Ptr tConcrete)
        {
            string domain = tConcrete.DomainSpec == null ? "this domain" : tConcrete.DomainSpec.Domain;

            return $"{_qualifierExplainer.Explain(tConcrete.Qualifier)} ip addresses mentioned in PTR record for {domain}.";
        }
    }
}