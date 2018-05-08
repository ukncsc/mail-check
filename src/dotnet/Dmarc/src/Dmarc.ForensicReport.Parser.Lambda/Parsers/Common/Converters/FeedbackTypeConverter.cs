using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IFeedbackTypeConverter : IEnumConverter<FeedbackType> { }

    public class FeedbackTypeConverter : EnumConverter<FeedbackType>, IFeedbackTypeConverter
    {
        public FeedbackTypeConverter(ILogger log) : base(log){}

        protected override bool TryConvert(string value, out FeedbackType? t)
        {
            value = value.Replace("-", string.Empty);
            return base.TryConvert(value, out t);
        }
    }
}