using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RuaTagsShouldBeMailTo : TagShouldBeMailTo<ReportUriAggregate>
    {
        public RuaTagsShouldBeMailTo()
            : base(DmarcRulesResource.RuaTagsShouldBeMailToErrorMessage)
        {
        }
    }
}
