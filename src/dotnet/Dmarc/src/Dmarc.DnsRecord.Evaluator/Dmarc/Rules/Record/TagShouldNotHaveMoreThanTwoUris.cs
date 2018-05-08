using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public abstract class TagShouldNotHaveMoreThanTwoUris<T> : IRule<DmarcRecord>
        where T : ReportUri
    {
        private readonly string _zeroUrisErrorMessage;
        private readonly string _moreThan2UrisErrorFormatString;

        protected TagShouldNotHaveMoreThanTwoUris(string zeroUrisErrorMessage, string moreThan2UrisErrorFormatString)
        {
            _zeroUrisErrorMessage = zeroUrisErrorMessage;
            _moreThan2UrisErrorFormatString = moreThan2UrisErrorFormatString;
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            T t = record.Tags.OfType<T>().FirstOrDefault();
            if (t == null || t.Uris.Count == 1 || t.Uris.Count == 2)
            {
                error = null;
                return false;
            }

            string errorMessage = t.Uris.Count == 0
                ? _zeroUrisErrorMessage
                : string.Format(_moreThan2UrisErrorFormatString, t.Uris.Count);

            error = new Error(ErrorType.Warning, errorMessage);
            return true;
        }
    }
}