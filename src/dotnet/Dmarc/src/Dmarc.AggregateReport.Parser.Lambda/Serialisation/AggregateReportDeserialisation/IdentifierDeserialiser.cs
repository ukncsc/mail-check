using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IIdentifiersDeserialiser
    {
        Identifier Deserialise(XElement identifiers);
    }

    public class IdentifiersDeserialiser : IIdentifiersDeserialiser
    {
        public Identifier Deserialise(XElement identifiers)
        {
            if (identifiers.Name != "identifiers")
            {
                throw new ArgumentException("Root element must be identifiers");
            }

            string envelopeTo = identifiers.SingleOrDefault("envelope_to")?.Value;
            string envelopeFrom = identifiers.SingleOrDefault("envelope_from")?.Value;
            string headerFrom = identifiers.Single("header_from").Value;

            return new Identifier(envelopeTo, envelopeFrom, headerFrom);
        }
    }
}