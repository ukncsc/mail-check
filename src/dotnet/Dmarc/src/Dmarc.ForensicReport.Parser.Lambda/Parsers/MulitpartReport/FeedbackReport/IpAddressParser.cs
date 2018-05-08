using System.Linq;
using System.Net;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IIpAddressParser : IHeaderParser<IPAddress> { }

    public class IpAddressParser : HeaderParserSingle<IPAddress>, IIpAddressParser
    {
        private readonly IIPAddressConverter _ipAddressConverter;

        public IpAddressParser(IIPAddressConverter ipAddressConverter)
        {
            _ipAddressConverter = ipAddressConverter;
        }

        protected override IPAddress Convert(string value, string fieldName, bool parseMandatory)
        {
            value = value.Split(' ').FirstOrDefault();
            return _ipAddressConverter.Convert(value, fieldName, parseMandatory);
        }
    }
}
