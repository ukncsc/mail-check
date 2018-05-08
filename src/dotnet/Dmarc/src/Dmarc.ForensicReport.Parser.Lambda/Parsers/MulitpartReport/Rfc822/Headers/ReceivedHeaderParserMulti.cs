using System.Collections.Generic;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers
{
    public interface IReceivedHeaderParserMulti : IHeaderParser<List<ReceivedHeader>> { }

    public class ReceivedHeaderParserMulti : HeaderParserMulti<ReceivedHeader, List<ReceivedHeader>>, IReceivedHeaderParserMulti
    {
        private readonly IReceivedHeaderConverter _receivedHeaderConverter;

        public ReceivedHeaderParserMulti(IReceivedHeaderConverter receivedHeaderConverter)
        {
            _receivedHeaderConverter = receivedHeaderConverter;
        }

        protected override ReceivedHeader Convert(string value, string fieldName, bool parseMandatory)
        {
            return _receivedHeaderConverter.Convert(value, fieldName, parseMandatory);
        }
    }
}
