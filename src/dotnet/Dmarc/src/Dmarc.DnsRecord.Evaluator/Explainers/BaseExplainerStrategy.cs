using System;

namespace Dmarc.DnsRecord.Evaluator.Explainers
{
    public abstract class BaseExplainerStrategy<TBase, TConcrete> : IExplainerStrategy<TBase>
        where TConcrete : class, TBase
    {
        public abstract bool TryExplain(TBase t, out string explanation);

        public Type Type => typeof(TConcrete);

        protected TConcrete ToTConcrete(TBase t)
        {
            TConcrete tConcrete = t as TConcrete;
            if (tConcrete == null)
            {
                throw new ArgumentException($"{GetType()} expects type of {Type} but got type of {t?.GetType().ToString() ?? "<null>"}");
            }
            return tConcrete;
        }
    }
}