using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class MxTermExplainer : BaseTermExplainerStrategy<Mx>
    {
        private readonly IQualifierExplainer _qualifierExplainer;

        public MxTermExplainer(IQualifierExplainer qualifierExplainer)
        {
            _qualifierExplainer = qualifierExplainer;
        }

        public override string GetExplanation(Mx tConcrete)
        {
            string domain = tConcrete.DomainSpec?.Domain ?? "this domain";

            return string.Format(SpfExplainerResource.MxExplanation, _qualifierExplainer.Explain(tConcrete.Qualifier), domain,
                tConcrete.DualCidrBlock.Ip4CidrBlock.Value.ToString() ?? "invalid", tConcrete.DualCidrBlock.Ip6CidrBlock.Value.ToString() ?? "invalid");
        }
    }
}