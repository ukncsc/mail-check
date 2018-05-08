using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Rules.Config
{
    public class OnlyOneSpfRecord : IRule<SpfConfig>
    {
        public bool IsErrored(SpfConfig spfConfig, out Error error)
        {
            int recordCount = spfConfig.Records.Count;
            if (recordCount == 1)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(SpfRulesResource.OnlyOneSpfRecordErrorMessage, recordCount);

            error = new Error(ErrorType.Error, errorMessage);
            return true;
        }
    }
}