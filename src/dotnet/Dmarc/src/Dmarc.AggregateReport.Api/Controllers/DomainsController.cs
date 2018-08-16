using System.Security.Claims;
using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Dao.Domain;
using Dmarc.AggregateReport.Api.Domain;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Api.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dmarc.Common.Api.Domain;

namespace Dmarc.AggregateReport.Api.Controllers
{
    [Route("api/aggregatereport")]
    public class DomainsController : Controller
    {
        private readonly IDomainsDao _domainsDao;
        private readonly IValidator<DomainSearchRequest> _domainSearchValidator;
        private readonly ILogger _log;

        public DomainsController(IDomainsDao domainsDao,
            IValidator<DomainSearchRequest> domainSearchValidator,
            ILogger<DomainsController> log)
        {
            _domainsDao = domainsDao;
            _domainSearchValidator = domainSearchValidator;
            _log = log;
        }

        [HttpGet]
        [Route("domains")]
        public async Task<IActionResult> GetDomains(DomainSearchRequest domainSearch)
        {
            ValidationResult validationResult = await _domainSearchValidator.ValidateAsync(domainSearch);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            Claim roleClaim = User.FindFirst(_ => _.Type == ClaimTypes.Role);
            if (roleClaim.Value == RoleType.Unauthorised)
            {
                return Forbid();
            }

            int userId = GetUserId(User);
            MatchingDomains result = await _domainsDao.GetMatchingDomains(userId, domainSearch.SearchPattern);

            return new ObjectResult(result);
        }

        private int GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            Claim idClaim = claimsPrincipal.FindFirst(_ => _.Type == ClaimTypes.Sid);
            return int.Parse(idClaim.Value);
        }
    }
}
