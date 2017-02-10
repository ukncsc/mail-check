using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Common.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Common.Utils;

namespace Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation
{
    public interface IPolicyEvaluatedDeserialiser
    {
        PolicyEvaluated Deserialise(XElement policyEvaluated);
    }

    public class PolicyEvaluatedDeserialiser : IPolicyEvaluatedDeserialiser
    {
        private readonly IPolicyOverrideReasonDeserialiser _policyOverrideReasonDeserialiser;

        public PolicyEvaluatedDeserialiser(IPolicyOverrideReasonDeserialiser policyOverrideReasonDeserialiser)
        {
            _policyOverrideReasonDeserialiser = policyOverrideReasonDeserialiser;
        }

        public PolicyEvaluated Deserialise(XElement policyEvaluated)
        {
            if (policyEvaluated.Name != "policy_evaluated")
            {
                throw new ArgumentException("Root element must be policy_evaluated");
            }

            //nullable this deviates from spec
            //but some providers send values that are not in enum from the spec
            Disposition dispositionCandidate;
            Disposition? disposition = Enum.TryParse(policyEvaluated.SingleOrDefault("disposition")?.Value, true, out dispositionCandidate) ? dispositionCandidate : (Disposition?)null;

            //nullable this deviates from spec
            DmarcResult dkimCandidate;
            DmarcResult? dkim = Enum.TryParse(policyEvaluated.SingleOrDefault("dkim")?.Value, true, out dkimCandidate) ? dkimCandidate : (DmarcResult?)null;

            DmarcResult spf = (DmarcResult)Enum.Parse(typeof(DmarcResult), policyEvaluated.Single("spf")?.Value, true);

            
            //PolicyOverrideReason[] policyOverrideReasons = DeserialisePolicyOverrideReasons(policyEvaluated);
            PolicyOverrideReason[] policyOverrideReasons = _policyOverrideReasonDeserialiser.Deserialise(policyEvaluated.Where("reason"));

            return new PolicyEvaluated(disposition, dkim, spf, policyOverrideReasons);
        }
    }
}