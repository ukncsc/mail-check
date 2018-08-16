using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class CertificateEvaluatorApiRequest
    {
        public CertificateEvaluatorApiRequest(List<string> domains)
        {
            Domains = domains;
        }

        public List<string> Domains { get; }
    }
}
