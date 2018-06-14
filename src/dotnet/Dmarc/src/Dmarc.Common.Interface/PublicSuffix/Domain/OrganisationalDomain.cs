using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.Common.Interface.PublicSuffix.Domain
{
    public class OrganisationalDomain
    {
        public OrganisationalDomain(string orgDomain, string domain, bool isTld = false)
        {
            OrgDomain = orgDomain;
            Domain = domain;
            IsTld = isTld;

            if (isTld)
            {
                IsOrgDomain = false;
            }
            else
            {
                IsOrgDomain = OrgDomain == Domain;
            }
        }

        public string OrgDomain { get; }
        public string Domain { get; }
        public bool IsTld { get; }
        public bool IsOrgDomain { get; }
    }
}
