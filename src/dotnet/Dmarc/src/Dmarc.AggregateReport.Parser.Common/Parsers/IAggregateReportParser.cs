using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Common.Domain;

namespace Dmarc.AggregateReport.Parser.Common.Parsers
{
    public interface IAggregateReportParser
    {
        void Parse(EmailMessageInfo messageInfo);
    }

    public interface IAggregateReportParserAsync
    {
        Task Parse(EmailMessageInfo messageInfo);
    }
}