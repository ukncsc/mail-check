using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IRecordDeserialiser
    {
        Record[] Deserialise(IEnumerable<XElement> records);
    }

    public class RecordDeserialiser : IRecordDeserialiser
    {
        private readonly IRowDeserialiser _rowDeserialiser;
        private readonly IIdentifiersDeserialiser _identifiersDeserialiser;
        private readonly IAuthResultDeserialiser _authResultDeserialiser;

        public RecordDeserialiser(IRowDeserialiser rowDeserialiser,
            IIdentifiersDeserialiser identifiersDeserialiser,
            IAuthResultDeserialiser authResultDeserialiser)
        {
            _rowDeserialiser = rowDeserialiser;
            _identifiersDeserialiser = identifiersDeserialiser;
            _authResultDeserialiser = authResultDeserialiser;
        }
        
        public Record[] Deserialise(IEnumerable<XElement> records)
        {
            if (!records.Any(_ => _.Name == "record"))
            {
                throw new ArgumentException("Aggregate report must contain at least 1 record.");
            }

            if (records.Any(_ => _.Name != "record"))
            {
                throw new ArgumentException("All elements must be records.");
            }

            return records.Select(CreateRecord).ToArray();
        }

        private Record CreateRecord(XElement record)
        {
            Row row = _rowDeserialiser.Deserialise(record.Single("row"));
            Identifier identifiers = _identifiersDeserialiser.Deserialise(record.Single("identifiers"));
            AuthResult authResults = _authResultDeserialiser.Deserialise(record.SingleOrDefault("auth_results"));
            return new Record(row, identifiers, authResults);
        }
    }
}