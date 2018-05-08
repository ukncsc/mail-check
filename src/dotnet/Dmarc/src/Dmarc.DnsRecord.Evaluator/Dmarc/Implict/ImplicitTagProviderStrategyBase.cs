using System;
using System.Collections.Generic;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Implicit;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public abstract class ImplicitTagProviderStrategyBase<TConcrete> 
        : ImplicitProviderStrategyBase<Tag, TConcrete>
        where TConcrete : Tag
    {
        protected ImplicitTagProviderStrategyBase(Func<List<Tag>, TConcrete> defaultValueFactory)
            :base(defaultValueFactory)
        {}   
    }
}