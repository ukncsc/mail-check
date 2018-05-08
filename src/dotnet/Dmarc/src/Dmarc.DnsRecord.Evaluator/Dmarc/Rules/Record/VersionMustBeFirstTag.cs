using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class VersionMustBeFirstTag : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            Tag firstTag = record.Tags.FirstOrDefault();

            if (firstTag is Version)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(DmarcRulesResource.VersionMustBeFirstTagErrorMessage, firstTag?.Value ?? "null");

            error = new Error(ErrorType.Error, errorMessage);
            return true;
        }
    }
}