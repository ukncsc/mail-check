using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class AspfImplicitProvider : ImplicitTagProviderStrategyBase<Aspf>
    {
        public AspfImplicitProvider() : base(t => Aspf.Default){}
    }
}