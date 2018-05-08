using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IDeliveryResultConverter : IEnumConverter<DeliveryResult>{}

    public class DeliveryResultConverter : EnumConverter<DeliveryResult>, IDeliveryResultConverter
    {
        public DeliveryResultConverter(ILogger log) : base(log){}
    }
}