using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IDeliveryResultParser : IHeaderParser<DeliveryResult?> { }

    public class DeliveryResultParser : EnumParser<DeliveryResult>, IDeliveryResultParser
    {
        public DeliveryResultParser(IDeliveryResultConverter converter) : base(converter){}
    }
}