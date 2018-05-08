using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class PolicyShouldBeQuarantineOrReject : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            Policy policy = record.Tags.OfType<Policy>().FirstOrDefault();

            //Dont error on unknown because there will already be a parser error for this
            if (policy == null || policy.PolicyType != PolicyType.None)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(DmarcRulesResource.PolicyShouldBeQuarantineOrRejectErrorMessage, policy.PolicyType);

            error = new Error(ErrorType.Warning, errorMessage);
            return true;
        }
    }
}