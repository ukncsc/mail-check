using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class AdkimImplicitProvider : ImplicitTagProviderStrategyBase<Adkim>
    {
        public AdkimImplicitProvider() : base(t => Adkim.Default){}
    }
}