using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IDkimAuthResultDeserialiser
    {
        DkimAuthResult[] Deserialise(IEnumerable<XElement> element);
    }

    public class DkimAuthResultDeserialiser : IDkimAuthResultDeserialiser
    {
        public DkimAuthResult[] Deserialise(IEnumerable<XElement> dkimAuthResults)
        {
            if (dkimAuthResults.Any(_ => _.Name != "dkim"))
            {
                throw new ArgumentException("All elements must be dkim.");
            }

            return dkimAuthResults.Select(Deserialise).ToArray();
        }

        private DkimAuthResult Deserialise(XElement element)
        {
            string domain = element.Single("domain").Value;

            string selector = element.SingleOrDefault("selector")?.Value;

            //nullable this deviates from spec
            //but some providers provide values not in the spec
            DkimResult dkimResultCandidate;
            DkimResult? dkimResult = Enum.TryParse(element.SingleOrDefault("result")?.Value, true, out dkimResultCandidate) ? dkimResultCandidate : (DkimResult?)null;

            string dkimHumanResult = element.SingleOrDefault("human_result")?.Value;

            return new DkimAuthResult(domain, selector, dkimResult, dkimHumanResult);
        }
    }
}