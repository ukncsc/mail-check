using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class WelcomeSearchResult
    {
        public WelcomeSearchResult(int id, string domainName)
        {
            Id = id;
            DomainName = domainName;
        }

        public int Id { get; }

        public string DomainName { get; }
    }
}
