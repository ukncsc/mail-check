using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class ExistsTermExplainer : BaseTermExplainerStrategy<Exists>
    {
        private readonly IQualifierExplainer _qualifierExplainer;

        public ExistsTermExplainer(IQualifierExplainer qualifierExplainer)
        {
            _qualifierExplainer = qualifierExplainer;
        }

        public override string GetExplanation(Exists tConcrete)
        {
            return string.Format(SpfExplainerResource.ExistsExplanation, _qualifierExplainer.Explain(tConcrete.Qualifier), tConcrete.DomainSpec.Domain);
        }
    }
}