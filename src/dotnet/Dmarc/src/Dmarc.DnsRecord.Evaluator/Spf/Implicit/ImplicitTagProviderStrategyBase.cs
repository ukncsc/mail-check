using System;
using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Implicit;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Implicit
{
    public abstract class ImplicitTagProviderStrategyBase<TConcrete> 
        : ImplicitProviderStrategyBase<Term, TConcrete>
        where TConcrete : Term
    {
        protected ImplicitTagProviderStrategyBase(Func<List<Term>, TConcrete> defaultValueFactory)
            :base(defaultValueFactory)
        {}   
    }
}