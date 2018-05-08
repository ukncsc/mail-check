using System.Collections.Generic;
using Dmarc.AggregateReport.Parser.Lambda.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Single
{
    public interface IDenormalisedRecordPersistor
    {
        void Persist(IEnumerable<DenormalisedRecord> denormalisedRecords);
    }
}