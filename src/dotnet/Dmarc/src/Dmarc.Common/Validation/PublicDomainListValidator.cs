using System.Collections.Generic;
using System.Linq;

namespace Dmarc.Common.Validation
{
    public interface IPublicDomainListValidator
    {
        bool IsValidPublicDomain(string domain);
    }

    public class PublicDomainListValidator : IPublicDomainListValidator
    {
        private readonly List<string> _publicDomainsList = new List<string>();

        public PublicDomainListValidator()
        {
            _publicDomainsList.AddRange(ValidationResources.PublicDomains.Split(',').Select(_ => _.Trim().ToLower()));
        }

        public bool IsValidPublicDomain(string domain) => _publicDomainsList.Any(_ => domain.ToLower().EndsWith($".{_}"));
    }
}
