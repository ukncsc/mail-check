using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class ReportIntervalImplicitProvider : ImplicitTagProviderStrategyBase<ReportInterval>
    {
        public ReportIntervalImplicitProvider() : base(t => ReportInterval.Default){}
    }
}