using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix.Domain;

namespace Dmarc.Common.Interface.PublicSuffix
{
    public interface IOrganisationalDomainProvider
    {
        Task<OrganisationalDomain> GetOrganisationalDomain(string domain);
    }
}
