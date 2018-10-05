using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Record
{
    public class DontUsePtrMechanism : IRule<SpfRecord>
    {
        public bool IsErrored(SpfRecord record, out Error error)
        {
            if (!record.Terms.OfType<Ptr>().Any())
            {
                error = null;
                return false;
            }

            error = new Error(ErrorType.Warning, SpfRulesResource.DontUsePtrMechanismErrorMessage);
            return true;
        }
    }
}