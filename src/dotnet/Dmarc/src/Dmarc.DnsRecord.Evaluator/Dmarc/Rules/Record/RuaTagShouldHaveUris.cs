using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RuaTagShouldHaveUris : TagShouldHaveUris<ReportUriAggregate>
    {
        public RuaTagShouldHaveUris()
            : base(DmarcRulesResource.RuaTagShouldHaveAtLeastOneUri)
        {
        }
    }
}
