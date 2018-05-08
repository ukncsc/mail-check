using Dmarc.Common.Api.Identity.Dao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders.Testing;

namespace Dmarc.Common.Api.Identity.Middleware
{
    public class IdentityMiddleware : AuthenticationMiddleware<AuthenticationOptions>
    {
        private readonly IIdentityDao _identityDao;
        private readonly ILogger<ClaimsHandler> _claimsHandlerLogger;

        public IdentityMiddleware(RequestDelegate next,
            IOptions<AuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            IIdentityDao identityDao)
            : base(next, options, loggerFactory, new UrlTestEncoder())
        {
            _identityDao = identityDao;
            _claimsHandlerLogger = loggerFactory.CreateLogger<ClaimsHandler>();
        }

        protected override AuthenticationHandler<AuthenticationOptions> CreateHandler()
        {
            return new ClaimsHandler(_identityDao, _claimsHandlerLogger);
        }
    }
}
