using System.Collections.Generic;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Persistence.Single;
using Dmarc.AggregateReport.Parser.Common.Serialisation;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.App.Persistence.Console
{
    internal class CsvDenormalisedRecordPersistor : IDenormalisedRecordPersistor
    {
        private readonly ICsvDenormalisedRecordSerialiser _serialiser;

        public CsvDenormalisedRecordPersistor(ICsvDenormalisedRecordSerialiser serialiser)
        {
            _serialiser = serialiser;
        }

        public void Persist(IEnumerable<DenormalisedRecord> denormalisedRecords)
        {
            denormalisedRecords.ForEach(_ => System.Console.WriteLine(_serialiser.Serialise(_)));
        }
    }
}
