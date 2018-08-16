using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Lambda.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Publishers
{
    public interface IMessagePublisher
    {
        Task Publish(AggregateReportInfo aggregateReportInfo);
    }
}