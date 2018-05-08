using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    internal class FailureReportingOptionsShouldBeOne : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            FailureOption failureOption = record.Tags.OfType<FailureOption>().FirstOrDefault();

            if (failureOption == null || failureOption.FailureOptionType == FailureOptionType.One)
            {
                error = null;
                return false;
            }

            string failureOptionString = GetFailureOptionString(failureOption.FailureOptionType);
            string errorMessasge = string.Format(DmarcRulesResource.FailureReportingOptionsShouldBeOneErrorMessage, failureOptionString);

            error = new Error(ErrorType.Warning, errorMessasge);
            return true;
        }

        private static string GetFailureOptionString(FailureOptionType failureOption)
        {
            switch (failureOption)
            {
                case FailureOptionType.Zero:
                    return "0";
                case FailureOptionType.One:
                    return "1";
                case FailureOptionType.D:
                    return "d";
                case FailureOptionType.S:
                    return "s";
            }
            return null;
        }
    }
}