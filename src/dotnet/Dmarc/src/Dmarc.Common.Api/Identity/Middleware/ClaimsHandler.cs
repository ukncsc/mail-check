using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dmarc.Common.Api.Identity.Dao;
using Dmarc.Common.Api.Identity.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Dmarc.Common.Api.Identity.Middleware
{
    public class ClaimsHandler : AuthenticationHandler<AuthenticationOptions>
    {
        private readonly IIdentityDao _identityDao;
        private readonly ILogger<ClaimsHandler> _log;

        public ClaimsHandler(IIdentityDao identityDao, ILogger<ClaimsHandler> log)
        {
            _identityDao = identityDao;
            _log = log;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _log.LogDebug($"Identity middleware handling request: {Context.Request.Path} ({Context.Request.Method})");

            IHeaderDictionary headerDictionary = Context.Request.Headers;
            StringValues email;
            if (!headerDictionary.TryGetValue(OidcClaims.Email, out email))
            {
                _log.LogError($"Request headers didnt contain header { OidcClaims.Email}");
                return AuthenticateResult.Fail(new InvalidOperationException($"Request headers doesnt contain header {OidcClaims.Email}"));
            }

            Domain.Identity identity = await _identityDao.GetIdentityByEmail(email);

            if (identity == null)
            {
                _log.LogDebug($"Creating user with email: {email}");

                StringValues name;
                if (!headerDictionary.TryGetValue(OidcClaims.GivenName, out name))
                {
                    _log.LogError($"Request headers didnt contain header {OidcClaims.GivenName}");
                    return
                        AuthenticateResult.Fail(
                            new InvalidOperationException($"Request headers doesnt contain header {OidcClaims.GivenName}"));
                }

                StringValues familyName;
                if (!headerDictionary.TryGetValue(OidcClaims.FamilyName, out familyName))
                {
                    _log.LogError($"Request headers didnt contain header {OidcClaims.FamilyName}");
                    return
                        AuthenticateResult.Fail(
                            new InvalidOperationException(
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

            return AuthenticateResult.Success(new AuthenticationTicket(principal,
                new AuthenticationProperties(), Options.AuthenticationScheme));
        }
    }
}