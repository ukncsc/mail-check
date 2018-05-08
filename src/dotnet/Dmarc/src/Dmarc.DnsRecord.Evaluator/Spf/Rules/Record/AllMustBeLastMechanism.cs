using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Record
{
    public class AllMustBeLastMechanism : IRule<SpfRecord>
    {
        public bool IsErrored(SpfRecord record, out Evaluator.Rules.Error error)
        {
            Mechanism lastMechanism = record.Terms.OfType<Mechanism>().LastOrDefault();

            if (lastMechanism == null || lastMechanism is All)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(SpfRulesResource.AllMustBeLastMechanismErrorMessage, lastMechanism.Value);

            error = new Evaluator.Rules.Error(Evaluator.Rules.ErrorType.Error, errorMessage);
            return true;
        }
    }
}