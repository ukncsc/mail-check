using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RufTagShouldNotHaveMoreThanTwoUris : TagShouldNotHaveMoreThanTwoUris<ReportUriForensic>
    {
        public RufTagShouldNotHaveMoreThanTwoUris()
            : base(DmarcRulesResource.RufTagShouldNotHaveMoreThanTwoUrisErrorMessage)
        {
        }
    }
}
