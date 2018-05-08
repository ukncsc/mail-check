using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public abstract class TagShouldBeMailTo<T> : IRule<DmarcRecord>
        where T : ReportUri
    {
        private readonly string _errorFormatString;
        private const string Scheme = "mailto";

        protected TagShouldBeMailTo(string errorFormatString)
        {
            _errorFormatString = errorFormatString;
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            T t = record.Tags.OfType<T>().FirstOrDefault();

            //ignore null uri schemes as these will already have parsing error.
            if (t == null || t.Uris.Select(_ => _.Uri.Uri?.Scheme).Where(_ => _ != null).All(_ => _ == Scheme))
            {
                error = null;
                return false;
            }

            error = new Error(ErrorType.Warning, _errorFormatString);
            return true;
        }
    }
}