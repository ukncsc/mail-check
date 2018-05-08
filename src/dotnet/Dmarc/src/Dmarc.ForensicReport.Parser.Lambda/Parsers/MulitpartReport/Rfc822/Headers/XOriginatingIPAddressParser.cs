using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers
{
    public interface IXOriginatingIPAddressParser : IHeaderParser<List<IPAddress>> {}

    public class XOriginatingIPAddressParser : HeaderParserMulti<IPAddress, List<IPAddress>>, IXOriginatingIPAddressParser
    {
        private readonly IIPAddressConverter _converter;

        public XOriginatingIPAddressParser(IIPAddressConverter converter)
        {
            _converter = converter;
        }

        protected override IPAddress Convert(string value, string fieldName, bool parseMandatory)
        {
            value = Regex.Replace(value, @"[\[\]]", "");

            return _converter.Convert(value, fieldName, parseMandatory);
        }
    }
}
