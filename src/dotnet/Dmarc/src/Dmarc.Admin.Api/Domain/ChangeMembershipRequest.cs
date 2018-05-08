using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Dmarc.Admin.Api.Domain
{
    public class ChangeMembershipRequest
    {
        public int Id { get; set; }

        [FromBody]
        public List<int> EntityIds { get; set; }
    }
}