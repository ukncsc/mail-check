using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public class SubDomainPolicyShouldNotBeOnNonOrganisationalDomain : IRule<DmarcRecord>
    {
        public bool IsErrored(DmarcRecord record, out Error error)
        {
            SubDomainPolicy subDomainPolicy = record.Tags.OfType<SubDomainPolicy>().FirstOrDefault();
            
            if (record.IsOrgDomain == false && subDomainPolicy != null && !subDomainPolicy.IsImplicit)
            {
                error = new Error(ErrorType.Warning, string.Format(DmarcRulesResource.SubDomainIneffectualErrorMessage, record.Domain));
                return true;
            }

            error = null;
            return false;
        }
    }
}
