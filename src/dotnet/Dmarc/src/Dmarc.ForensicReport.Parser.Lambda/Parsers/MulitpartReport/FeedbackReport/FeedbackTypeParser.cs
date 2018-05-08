using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IFeedbackTypeParser : IHeaderParser<FeedbackType?>{}

    public class FeedbackTypeParser : EnumParser<FeedbackType>, IFeedbackTypeParser
    {
        public FeedbackTypeParser(IFeedbackTypeConverter converter) : base(converter){}
    }
}