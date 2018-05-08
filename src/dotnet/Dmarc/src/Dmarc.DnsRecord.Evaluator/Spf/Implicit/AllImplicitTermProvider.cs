using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Implicit
{
    public class AllImplicitTermProvider : ImplicitTermProviderStrategyBase<All>
    {
        public AllImplicitTermProvider() 
            : base(t => All.Default)
        {
        }
    }
}