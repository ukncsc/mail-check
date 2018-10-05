using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Config
{
    public class OnlyOneDmarcRecord : IRule<DmarcConfig>
    {
        public bool IsErrored(DmarcConfig config, out Error error)
        {
            int recordCount = config.Records.Count;

            if (config.IsTld || recordCount == 1)
            {
                error = null;
                return false;
            }

            string message = recordCount == 0
                ? string.Format(DmarcRulesResource.NoDmarcErrorMessage, config.Domain)
                : DmarcRulesResource.OnlyOneDmarcRecordErrorMessage;

            error = new Error(ErrorType.Error, message);

            return true;
        }
    }
}
