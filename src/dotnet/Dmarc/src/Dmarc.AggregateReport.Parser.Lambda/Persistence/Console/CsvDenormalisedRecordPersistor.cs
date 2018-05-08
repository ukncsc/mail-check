using System.Collections.Generic;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Console
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
