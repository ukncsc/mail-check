using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Explainers
{
    public abstract class BaseTermExplainerStrategy<TConcrete> :  BaseExplainerStrategy<Term, TConcrete>
        where TConcrete : Term
    {
        public override bool TryExplain(Term t, out string explanation)
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
