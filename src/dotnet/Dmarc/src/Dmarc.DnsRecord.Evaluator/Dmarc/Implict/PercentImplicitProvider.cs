using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class PercentImplicitProvider : ImplicitTagProviderStrategyBase<Percent>
    {
        public PercentImplicitProvider() : base(t => Percent.Default){}
    }
}