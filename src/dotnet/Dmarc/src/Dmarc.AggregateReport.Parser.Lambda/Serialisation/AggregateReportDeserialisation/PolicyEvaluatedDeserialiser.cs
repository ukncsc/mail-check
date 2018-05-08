using System;
using System.Linq;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
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

            var valid = new[] { "disposition", "dkim", "spf" };
            var children = policyEvaluated.Elements().Select(_ => _.Name.ToString());
            if (!children.Intersect(valid).Any())
            {
                throw new InvalidOperationException("policy_evaluated does not contain disposition dkim or spf");
            }

            // disposition, dkim and spf are nullable which deviates from spec
            // but some providers send values that are not in enum from the spec
            Disposition dispositionCandidate;
            Disposition? disposition = Enum.TryParse(policyEvaluated.SingleOrDefault("disposition")?.Value, true, out dispositionCandidate) ? dispositionCandidate : (Disposition?)null;

            DmarcResult dkimCandidate;
            DmarcResult? dkim = Enum.TryParse(policyEvaluated.SingleOrDefault("dkim")?.Value, true, out dkimCandidate) ? dkimCandidate : (DmarcResult?)null;

            DmarcResult spfCandidate;
            DmarcResult? spf = Enum.TryParse(policyEvaluated.SingleOrDefault("spf")?.Value, true, out spfCandidate) ? spfCandidate : (DmarcResult?)null;

            PolicyOverrideReason[] policyOverrideReasons = _policyOverrideReasonDeserialiser.Deserialise(policyEvaluated.Where("reason"));

            return new PolicyEvaluated(disposition, dkim, spf, policyOverrideReasons);
        }
    }
}
