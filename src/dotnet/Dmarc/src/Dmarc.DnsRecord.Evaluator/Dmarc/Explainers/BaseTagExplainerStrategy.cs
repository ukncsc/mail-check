using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Explainers;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Explainers
{
    public abstract class BaseTagExplainerStrategy<TConcrete> : BaseExplainerStrategy<Tag, TConcrete>
        where TConcrete : Tag
    {
        public override bool TryExplain(Tag t, out string explanation)
        {
            TConcrete concrete = ToTConcrete(t);

            if (concrete.AllValid)
            {
                explanation = GetExplanation(concrete);
                return true;
            }
            explanation = null;
            return false;
        }

        public abstract string GetExplanation(TConcrete tConcrete);
    }
}