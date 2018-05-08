using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IIntConverter : IConverter<int?>{}

    public class IntConverter : Converter<int?>, IIntConverter
    {
        public IntConverter(ILogger log) : base(log){}

        protected override bool TryConvert(string value, out int? t)
        {
            int intValue;
            if (int.TryParse(value, out intValue))
            {
                t = intValue;
                return true;
            }
            t = null;
            return false;
        }
    }
}
