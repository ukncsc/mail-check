using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using System;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RuaTagsShouldContainDmarcServiceMailBox : TagsShouldContainDmarcServiceMailBox<ReportUriAggregate>
    {
        public RuaTagsShouldContainDmarcServiceMailBox()
            : base(DmarcRulesResource.RuaTagsShouldContainDmarcServiceMailBoxErrorMessage,
                DmarcRulesResource.RuaTagShouldNotHaveMisconfiguredMailCheckMailboxErrorMessage,
                DmarcRulesResource.RuaTagShouldNotContainDuplicateUrisErrorMessage,
                new Uri(DmarcRulesResource.RuaMailbox))
        {
        }
    }
}
