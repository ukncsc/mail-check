using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class ATermExplainer : BaseTermExplainerStrategy<A>
    {
        private readonly IQualifierExplainer _qualifierExplainer;

        public ATermExplainer(IQualifierExplainer qualifierExplainer)
        {
            _qualifierExplainer = qualifierExplainer;
        }

        public override string GetExplanation(A tConcrete)
        {
            string domain = tConcrete.DomainSpec?.Domain ?? "this domain";

            return string.Format(SpfExplainerResource.AExplanation, _qualifierExplainer.Explain(tConcrete.Qualifier), domain,
               tConcrete.DualCidrBlock.Ip4CidrBlock.Value.ToString() ?? "invalid", tConcrete.DualCidrBlock.Ip6CidrBlock.Value.ToString() ?? "invalid");
        }
    }
}