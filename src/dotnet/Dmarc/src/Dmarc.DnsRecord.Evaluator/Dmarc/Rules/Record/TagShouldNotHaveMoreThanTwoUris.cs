using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public abstract class TagShouldNotHaveMoreThanTwoUris<T> : IRule<DmarcRecord>
        where T : ReportUri
    {
        private readonly string _moreThan2UrisErrorFormatString;

        protected TagShouldNotHaveMoreThanTwoUris(string moreThan2UrisErrorFormatString)
        {
            _moreThan2UrisErrorFormatString = moreThan2UrisErrorFormatString;
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            T t = record.Tags.OfType<T>().FirstOrDefault();
            if (t == null || t.Uris.Count < 3)
            {
                error = null;
                return false;
            }

            error = new Error(ErrorType.Warning, string.Format(_moreThan2UrisErrorFormatString, t.Uris.Count));

            return true;
        }
    }
}
