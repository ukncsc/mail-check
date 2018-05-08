using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IPolicyOverrideReasonDeserialiser
    {
        PolicyOverrideReason[] Deserialise(IEnumerable<XElement> reasons);
    }

    public class PolicyOverrideReasonDeserialiser : IPolicyOverrideReasonDeserialiser
    {
        public PolicyOverrideReason[] Deserialise(IEnumerable<XElement> reasons)
        {
            if (reasons.Any(_ => _.Name != "reason"))
            {
                throw new ArgumentException("All elements must be reason.");
            }

            return reasons.Select(Deserialise).ToArray();
        }

        private  PolicyOverrideReason Deserialise(XElement reason)
        {
            //nullable this deviates from spec
            //but some providers provide values not in the spec
            PolicyOverride typeCandidate;
            PolicyOverride? type = Enum.TryParse(reason.SingleOrDefault("type")?.Value, true, out typeCandidate) ? typeCandidate : (PolicyOverride?)null;

            string comment = reason.SingleOrDefault("comment")?.Value;

            return new PolicyOverrideReason(type, comment);
        }
    }
}