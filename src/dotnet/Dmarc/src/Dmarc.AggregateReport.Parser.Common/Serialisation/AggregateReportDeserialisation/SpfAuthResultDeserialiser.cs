using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Common.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Common.Utils;

namespace Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation
{
    public interface ISpfAuthResultDeserialiser
    {
        SpfAuthResult[] Deserialise(IEnumerable<XElement> spfAuthResults);
    }

    public class SpfAuthResultDeserialiser : ISpfAuthResultDeserialiser
    {
        public SpfAuthResult[] Deserialise(IEnumerable<XElement> spfAuthResults)
        {
            if (spfAuthResults.Any(_ => _.Name != "spf"))
            {
                throw new ArgumentException("All elements must be spf.");
            }

            return spfAuthResults.Select(Deserialise).ToArray();
        }

        private SpfAuthResult Deserialise(XElement element)
        {
            string domain = element.Single("domain").Value;

            SpfResult candidateResult;
            //Single as expecting to get the element, nullable as the result from the element might not be in the enum from the spec.
            SpfResult? spfResult = Enum.TryParse(element.Single("result").Value, true, out candidateResult) ? candidateResult : (SpfResult?)null;

            return new SpfAuthResult(domain, spfResult);
        }
    }
}