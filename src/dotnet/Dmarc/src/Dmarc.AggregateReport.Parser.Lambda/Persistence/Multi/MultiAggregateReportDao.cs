using System;
using System.Collections.Generic;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Dmarc.Common.Linq;
using Dmarc.Common.Report.Persistance;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Multi
{
    public class MultiAggregateReportPersistor : IReportPersistor<AggregateReportInfo>, IDisposable
    {
        private readonly IEnumerable<IDenormalisedRecordPersistor> _persistors;
        private readonly IDenormalisedRecordConverter _denormalisedRecordConverter;

        public MultiAggregateReportPersistor(IEnumerable<IDenormalisedRecordPersistor> persistors,
            IDenormalisedRecordConverter denormalisedRecordConverter)
        {
            _persistors = persistors;
            _denormalisedRecordConverter = denormalisedRecordConverter;
        }

        public void Persist(AggregateReportInfo report)
        {
            List<DenormalisedRecord> denormalisedRecords = _denormalisedRecordConverter.ToDenormalisedRecord(report.AggregateReport, report.EmailMetadata.OriginalUri);
            _persistors.ForEach(_ => _.Persist(denormalisedRecords));
        }

        public void Dispose()
        {
            foreach (var persistor in _persistors)
            {
                (persistor as IDisposable)?.Dispose();
            }
        }
    }
}
