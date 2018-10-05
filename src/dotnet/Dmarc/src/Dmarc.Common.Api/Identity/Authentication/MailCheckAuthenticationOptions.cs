using Microsoft.AspNetCore.Authentication;

namespace Dmarc.Common.Api.Identity.Authentication
{
    public class MailCheckAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string AuthenticationScheme => "Automatic";
    }
}
