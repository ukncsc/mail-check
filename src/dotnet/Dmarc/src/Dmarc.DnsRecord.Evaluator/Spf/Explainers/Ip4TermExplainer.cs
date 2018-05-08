using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public class Ip4TermExplainer : BaseTermExplainerStrategy<Ip4>
    {
        private readonly IQualifierExplainer _qualifierExplainer;

        public Ip4TermExplainer(IQualifierExplainer qualifierExplainer)
        {
            _qualifierExplainer = qualifierExplainer;
        }

        public override string GetExplanation(Ip4 tConcrete)
        {
            return string.Format(SpfExplainerResource.IpExplanation, _qualifierExplainer.Explain(tConcrete.Qualifier),
                tConcrete.IpAddress, tConcrete.CidrBlock.Value.ToString() ?? "invalid");
        }
    }
}