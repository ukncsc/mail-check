using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Record
{
    public class RedirectModifierAndAllMechanismNotValid : IRule<SpfRecord>
    {
        public bool IsErrored(SpfRecord record, out Error error)
        {
            if (record.Terms.OfType<All>().Any(_ => !_.IsImplicit) && record.Terms.OfType<Redirect>().Any())
            {
                error = new Error(ErrorType.Error,
                    SpfRulesResource.RedirectModifierAndAllMechanismNotValidErrorMessage);
                return true;
            }

            error = null;
            return false;
        }
    }
}