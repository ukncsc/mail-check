using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IAuthFailureParser : IHeaderParser<AuthFailure?> { }

    public class AuthFailureParser : EnumParser<AuthFailure>, IAuthFailureParser
    {
        public AuthFailureParser(IAuthFailureConverter converter) : base(converter){ }
    }
}