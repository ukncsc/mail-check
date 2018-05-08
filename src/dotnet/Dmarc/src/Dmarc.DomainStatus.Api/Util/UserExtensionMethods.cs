using System.Security.Claims;

namespace Dmarc.DomainStatus.Api.Util
{
    public static class UserExtensionMethods
    {
        public static int? GetId(this ClaimsPrincipal user)
        {
            string sid = user.FindFirst(_ => _.Type == ClaimTypes.Sid)?.Value;
            int id;
            return int.TryParse(sid, out id) ? id : (int?)null;
        }
    }
}
