using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Config
{
    public class TldDmarcRecordBehaviourIsWeaklyDefined : IRule<DmarcConfig>
    {
        public bool IsErrored(DmarcConfig config, out Error error)
        {
            if (config.IsTld && config.Records.Count > 0)
            {
                error = new Error(ErrorType.Warning, string.Format(DmarcRulesResource.TldDmarcWeaklyDefinedMessage, config.Domain));
                return true;
            }

            error = null;
            return false;
        }
    }
}
