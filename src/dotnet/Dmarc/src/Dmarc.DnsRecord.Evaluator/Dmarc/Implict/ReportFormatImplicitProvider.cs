using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Implict
{
    public class ReportFormatImplicitProvider : ImplicitTagProviderStrategyBase<ReportFormat>
    {
        public ReportFormatImplicitProvider() : base(t => ReportFormat.Default){}
    }
}