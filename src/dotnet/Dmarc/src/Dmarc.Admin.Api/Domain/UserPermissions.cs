using System.Collections.Generic;

namespace Dmarc.Admin.Api.Domain
{
    public class UserPermissions
    {
        public UserPermissions(User user, List<DomainPermission> domainPermissions)
        {
            User = user;
            DomainPermissions = domainPermissions;
        }

        public User User { get; }
        public List<DomainPermission> DomainPermissions { get; }
    }
}