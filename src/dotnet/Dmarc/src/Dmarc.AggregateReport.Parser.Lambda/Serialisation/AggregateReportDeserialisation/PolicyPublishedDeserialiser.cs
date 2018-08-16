using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;
using Dmarc.Common.Validation;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IPolicyPublishedDeserialiser
    {
        PolicyPublished Deserialise(XElement policyPublished);
    }

    public class PolicyPublishedDeserialiser : IPolicyPublishedDeserialiser
    {
        private readonly IDomainValidator _domainValidator;

        public PolicyPublishedDeserialiser(IDomainValidator domainValidator)
        {
            _domainValidator = domainValidator;
        }

        public PolicyPublished Deserialise(XElement policyPublished)
        {
            if (policyPublished.Name != "policy_published")
            {
                throw new ArgumentException("Root element must be policy_published");
            }

            string domain = policyPublished.Single("domain").Value;
            if (!_domainValidator.IsValidDomain(domain))
            {
                throw new ArgumentException("Invalid domain");
            }

            //nullable this deviates from spec
            Alignment adkimCandidate;
            Alignment? adkim = Enum.TryParse(policyPublished.SingleOrDefault("adkim")?.Value, true, out adkimCandidate) ? adkimCandidate : (Alignment?)null;

            //nullable this deviates from spec
            Alignment aspfCandidate;
            Alignment? aspf = Enum.TryParse(policyPublished.SingleOrDefault("aspf")?.Value, true, out aspfCandidate) ? aspfCandidate : (Alignment?)null;

            Disposition p = (Disposition)Enum.Parse(typeof(Disposition), policyPublished.Single("p").Value, true);

            //nullable this deviates from spec
            Disposition spCandidate;
            Disposition? sp = Enum.TryParse(policyPublished.SingleOrDefault("sp")?.Value, true, out spCandidate) ? spCandidate : (Disposition?)null;

            //nullable this deviates from spec
            int pctCandidate;
            int? pct = int.TryParse(policyPublished.SingleOrDefault("pct")?.Value, out pctCandidate) ? pctCandidate : (int?)null;

            string fo = policyPublished.SingleOrDefault("fo")?.Value;

            return new PolicyPublished(domain, adkim, aspf, p, sp, pct, fo);
        }
    }
}