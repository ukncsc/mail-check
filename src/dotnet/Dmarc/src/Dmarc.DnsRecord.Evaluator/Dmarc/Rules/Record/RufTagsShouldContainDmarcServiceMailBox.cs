using System;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RufTagsShouldContainDmarcServiceMailBox : TagsShouldContainDmarcServiceMailBox<ReportUriForensic>
    {
        public RufTagsShouldContainDmarcServiceMailBox()
            : base(DmarcRulesResource.RufTagsShouldContainDmarcServiceMailBoxErrorMessage,
                DmarcRulesResource.RufTagShouldNotHaveMisconfiguredMailCheckMailboxErrorMessage,
                DmarcRulesResource.RufTagShouldNotContainDuplicateUrisErrorMessage,
                new Uri(DmarcRulesResource.RufMailbox))
        {
        }
    }
}
