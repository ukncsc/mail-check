using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Record
{
    // See RFC7208 3.4.  Record Size
    // We only store full record so can test if individual
    // string are greater than 255
    public class MaxLengthOf450Characters : IRule<SpfRecord>
    {
        private const int MaxRecordLength = 450;

        public bool IsErrored(SpfRecord record, out Error error)
        {
            int recordLength = record.Record.Length;
            if (recordLength <= MaxRecordLength)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(SpfRulesResource.MaxLengthOf450CharactersErrorMessage, MaxRecordLength, recordLength);
            error = new Error(ErrorType.Error, errorMessage);
            return true;
        }
    }
}