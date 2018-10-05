using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Record
{
    public class AllMustBeLastMechanism : IRule<SpfRecord>
    {
        public bool IsErrored(SpfRecord record, out Error error)
        {
            bool isRedirect = record.Terms.OfType<Redirect>().Any();
            Mechanism lastMechanism = record.Terms.OfType<Mechanism>().LastOrDefault();

            if (isRedirect || lastMechanism == null || lastMechanism is All)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(SpfRulesResource.AllMustBeLastMechanismErrorMessage, lastMechanism.Value);

            error = new Error(ErrorType.Error, errorMessage);
            return true;
        }
    }
}