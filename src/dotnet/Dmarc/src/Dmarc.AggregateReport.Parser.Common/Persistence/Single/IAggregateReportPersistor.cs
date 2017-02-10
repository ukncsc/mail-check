using System.Collections.Generic;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Common.Domain;

namespace Dmarc.AggregateReport.Parser.Common.Persistence.Single
{
    public interface IAggregateReportPersistor
    {
        void Persist(IEnumerable<AggregateReportInfo> aggregateReportInfo);

        Task PersistAsync(IEnumerable<AggregateReportInfo> aggregateReportInfo);
    }
}
