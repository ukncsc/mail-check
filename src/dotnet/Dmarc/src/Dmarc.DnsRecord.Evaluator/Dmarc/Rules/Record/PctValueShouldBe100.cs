using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class PctValueShouldBe100 : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            Percent percent = record.Tags.OfType<Percent>().FirstOrDefault();
            //when pct value is null there will be parser error so dont add more errors
            if (percent?.PercentValue == null || percent.PercentValue == 100)
            {
                error = null;
                return false;
            }

            string errorMessage = string.Format(DmarcRulesResource.PctValueShouldBe100ErrorMessage, percent.PercentValue);
            error = new Error(ErrorType.Warning, errorMessage);
            return true;
        }
    }
}