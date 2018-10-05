using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Dmarc.Common.Api.Identity.Dao;
using Dmarc.Common.Api.Identity.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Dmarc.Common.Api.Identity.Authentication
{
    public class MailCheckAuthenticationHandler : AuthenticationHandler<MailCheckAuthenticationOptions>
    {
        private readonly IIdentityDao _identityDao;
        private readonly ILogger<MailCheckAuthenticationHandler> _log;

        public MailCheckAuthenticationHandler(
            IOptionsMonitor<MailCheckAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IIdentityDao identityDao,
            ILogger<MailCheckAuthenticationHandler> log) : base(options, logger, encoder, clock)
        {
            _identityDao = identityDao;
            _log = log;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _log.LogDebug($"Identity middleware handling request: {Context.Request.Path} ({Context.Request.Method})");

            IHeaderDictionary headerDictionary = Context.Request.Headers;

            if (!headerDictionary.TryGetValue(OidcClaims.Email, out StringValues email))
            {
                _log.LogError($"Request headers didnt contain header { OidcClaims.Email}");

                return AuthenticateResult.Fail(new InvalidOperationException(
                    $"Request headers doesnt contain header {OidcClaims.Email}"));
            }

            Domain.Identity identity = await _identityDao.GetIdentityByEmail(email);

            if (identity == null)
            {
                _log.LogDebug($"Creating user with email: {email}");

                if (!headerDictionary.TryGetValue(OidcClaims.GivenName, out StringValues name))
                {
                    _log.LogError($"Request headers didnt contain header {OidcClaims.GivenName}");

                    return AuthenticateResult.Fail(new InvalidOperationException(
                        $"Request headers doesnt contain header {OidcClaims.GivenName}"));
                }

                if (!headerDictionary.TryGetValue(OidcClaims.FamilyName, out StringValues familyName))
                {
                    _log.LogError($"Request headers didnt contain header {OidcClaims.FamilyName}");

                    return AuthenticateResult.Fail(new InvalidOperationException(
                        $"Request headers doesnt contain header {OidcClaims.FamilyName}"));
                }

                identity = await _identityDao.CreateIdentity(new IdentityForCreation(
                    name,
                    familyName,
                    email,
                    RoleType.Standard));

                _log.LogDebug($"Successfully created user {name} {familyName} ({email}).");
            }
            else
            {
                _log.LogDebug($"Found user {identity.FirstName} {identity.LastName} ({email}).");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, identity.RoleType),
                new Claim(ClaimTypes.Surname, identity.LastName),
                new Claim(ClaimTypes.Name, identity.FirstName),
                new Claim(ClaimTypes.Sid, identity.Id.ToString())
            };

            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            return AuthenticateResult.Success(new AuthenticationTicket(
                principal,
                new AuthenticationProperties(),
                Options.AuthenticationScheme));
        }
    }

}
