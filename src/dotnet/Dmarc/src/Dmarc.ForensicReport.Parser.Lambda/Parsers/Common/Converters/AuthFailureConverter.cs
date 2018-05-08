using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters
{
    public interface IAuthFailureConverter : IEnumConverter<AuthFailure>{}

    public class AuthFailureConverter : EnumConverter<AuthFailure>, IAuthFailureConverter
    {
        public AuthFailureConverter(ILogger log) : base(log){}
    }
}