using System.Collections.Generic;
using System.Linq;

namespace Dmarc.Admin.Api.Domain
{
    public class SearchResult
    {
        public SearchResult(Result<List<Domain>> domain, Result<List<User>> user, Result<List<Group>> group)
        {
            Domain = domain;
            User = user;
            Group = group;
        }

        public Result<List<Domain>> Domain { get; }

        public Result<List<User>> User { get; }

        public Result<List<Group>> Group { get; }

        //Required for json serialization - removes result if null or empty.
        public bool ShouldSerializeDomain()
        {
            return Domain != null && Domain.Results.Any();
        }

        public bool ShouldSerializeUser()
        {
            return User != null && User.Results.Any();
        }

        public bool ShouldSerializeGroup()
        {
            return Group != null && Group.Results.Any();
        }
    }
}