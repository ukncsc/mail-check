using System.Net;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IIPAddressConverter : IConverter<IPAddress> { }

    public class IPAddressConverter : Converter<IPAddress>, IIPAddressConverter
    {
        public IPAddressConverter(ILogger log) : base(log){}

        protected override bool TryConvert(string value, out IPAddress t)
        {
            return IPAddress.TryParse(value, out t);
        }
    }
}
