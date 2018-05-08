using Dmarc.Common.Report.Domain;

namespace Dmarc.Common.Report.Parser
{
    public interface IReportParser<out T>
    {
        T Parse(EmailMessageInfo messageInfo);
    }
}