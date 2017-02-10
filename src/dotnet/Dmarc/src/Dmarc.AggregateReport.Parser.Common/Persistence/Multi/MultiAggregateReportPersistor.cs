using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Persistence.Single;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.Common.Persistence.Multi
{
    public interface IMultiAggregateReportPersistor : IAggregateReportPersistor
    {
        int Count { get; }

        bool Active { get; }
    }

    public class MultiAggregateReportPersistor : IMultiAggregateReportPersistor
    {
        private readonly IEnumerable<IAggregateReportPersistor> _persistors;

        public MultiAggregateReportPersistor(IEnumerable<IAggregateReportPersistor> persistors)
        {
            _persistors = persistors;
        }

        public void Persist(IEnumerable<AggregateReportInfo> aggregateReportInfo)
        {
            _persistors.ForEach(_ => _.Persist(aggregateReportInfo));
        }

        public Task PersistAsync(IEnumerable<AggregateReportInfo> aggregateReportInfo)
        {
            throw new NotImplementedException();
        }

        public int Count => _persistors.Count();

        public bool Active => _persistors.Any();
    }
}
