using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class PolicyTagMustExist : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            if (record.Tags.OfType<Policy>().Any())
            {
                error = null;
                return false;
            }

            error = new Error(ErrorType.Error, DmarcRulesResource.PolicyTagMustExistErrorMessage);
            return true;
        }
    }
}