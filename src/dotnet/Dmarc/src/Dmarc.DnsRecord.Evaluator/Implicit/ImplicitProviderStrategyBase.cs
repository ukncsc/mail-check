using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Implicit
{
    public abstract class ImplicitProviderStrategyBase<TBase, TConcrete> : IImplicitProviderStrategy<TBase> 
        where TConcrete : TBase
        where TBase : class 
    {
        private readonly Func<List<TBase>, TConcrete> _defaultValueFactory;

        protected ImplicitProviderStrategyBase(Func<List<TBase>, TConcrete> defaultValueFactory)
        {
            _defaultValueFactory = defaultValueFactory;
        }

        public bool TryGetImplicitTag(List<TBase> ts, out TBase t)
        {
            if (ts.OfType<TConcrete>().Any())
            {
                t = null;
                return false;
            }

            t = _defaultValueFactory(ts);
            return true;
        }
    }
}