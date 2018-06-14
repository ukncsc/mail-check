using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class RufTagShouldBeMailTo : TagShouldBeMailTo<ReportUriForensic>
    {
        public RufTagShouldBeMailTo()
            : base(DmarcRulesResource.RufTagShouldBeMailToErrorMessage)
        {
        }
    }
}
