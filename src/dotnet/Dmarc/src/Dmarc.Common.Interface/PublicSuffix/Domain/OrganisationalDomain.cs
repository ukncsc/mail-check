using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.Common.Interface.PublicSuffix.Domain
{
    public class OrganisationalDomain
    {
        public OrganisationalDomain(string orgDomain, string domain)
        {
            OrgDomain = orgDomain;
            Domain = domain;
        }

        public string OrgDomain { get; }
        public string Domain { get; }
        public bool IsOrgDomain => OrgDomain == Domain;
    }
}
