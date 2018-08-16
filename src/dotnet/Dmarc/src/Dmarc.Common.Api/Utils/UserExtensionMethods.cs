using Dmarc.Common.Api.Identity.Domain;
using System.Linq;
using System.Security.Claims;

namespace Dmarc.Common.Api.Utils
{
    public static class UserExtensionMethods
    {
        public static int? GetId(this ClaimsPrincipal user)
        {
            string sid = user.FindFirst(_ => _.Type == ClaimTypes.Sid)?.Value;
            int id;
            return int.TryParse(sid, out id) ? id : (int?)null;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.Claims.Any(_ => _.Type == ClaimTypes.Role && _.Value == RoleType.Admin);
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
        }
    }
}
