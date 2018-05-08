using Dmarc.AggregateReport.Parser.Lambda.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Single
{
    public interface IAggregateReportPersistor
    {
        void Persist(AggregateReportInfo aggregateReportInfo);
    }
}
