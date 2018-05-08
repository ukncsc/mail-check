using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class FailureOptionsImplicitProvider : ImplicitTagProviderStrategyBase<FailureOption>
    {
        public FailureOptionsImplicitProvider() : base(t => FailureOption.Default){}
    }
}