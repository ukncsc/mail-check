using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Record
{
    public class RedirectDoesntOccurMoreThanOnce : IRule<SpfRecord>
    {
        public bool IsErrored(SpfRecord record, out Error error)
        {
            int redirectCount = record.Terms.OfType<Redirect>().Count();
            if (redirectCount <= 1)
            {
                error = null;
                return false;
            }

            error = new Error(ErrorType.Error, string.Format(SpfRulesResource.RedirectDoesntOccurMoreThanOnceErrorMessage, redirectCount));
            return true;
        }
    }
}