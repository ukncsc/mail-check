using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public abstract class TagShouldHaveUris<T> : IRule<DmarcRecord> where T : ReportUri
    {
        private readonly string _zeroUrisErrorMessage;

        protected TagShouldHaveUris(string zeroUrisErrorMessage)
        {
            _zeroUrisErrorMessage = zeroUrisErrorMessage;
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            T t = record.Tags.OfType<T>().FirstOrDefault();
            if (t == null || t.Uris.Any())
            {
                error = null;
                return false;
            }

            error = new Error(ErrorType.Warning, _zeroUrisErrorMessage);

            return true;
        }
    }
}
