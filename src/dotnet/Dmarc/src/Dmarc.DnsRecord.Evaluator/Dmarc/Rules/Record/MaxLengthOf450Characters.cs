using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class MaxLengthOf450Characters : IRule<DmarcRecord>
    {
        private const int MaxRecordLength = 450;

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            int recordLength = record.Record.Length;
            if (recordLength <= MaxRecordLength)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(DmarcRulesResource.MaxLengthOf450CharactersErrorMessage, MaxRecordLength, recordLength);
            error = new Error(ErrorType.Error, errorMessage);
            return true;
        }
    }
}