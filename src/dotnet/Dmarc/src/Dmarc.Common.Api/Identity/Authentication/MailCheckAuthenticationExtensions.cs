using System;
using Microsoft.AspNetCore.Authentication;

namespace Dmarc.Common.Api.Identity.Authentication
{
    public static class MailCheckAuthenticationExtensions
    {
        public static AuthenticationBuilder AddMailCheckAuthentication(this AuthenticationBuilder builder, Action<MailCheckAuthenticationOptions> configureOptions = null)
        {
            return builder.AddScheme<MailCheckAuthenticationOptions, MailCheckAuthenticationHandler>(
                "Automatic",
                "AutomaticMailCheckAuthentication",
                configureOptions ?? (options => {}));
        }


    }
}
