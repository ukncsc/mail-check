using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RuaTagShouldNotHaveMoreThanTwoUris : TagShouldNotHaveMoreThanTwoUris<ReportUriAggregate>
    {
        public RuaTagShouldNotHaveMoreThanTwoUris()
            : base(DmarcRulesResource.RuaTagShouldNotHaveMoreThanTwoUrisErrorMessage)
        {
        }
    }
}
