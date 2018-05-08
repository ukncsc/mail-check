namespace Dmarc.Common.Api.Identity
{
    public class AuthenticationOptions : Microsoft.AspNetCore.Builder.AuthenticationOptions
    {
        public AuthenticationOptions()
        {
            AuthenticationScheme = "Automatic";
        }
    }
}