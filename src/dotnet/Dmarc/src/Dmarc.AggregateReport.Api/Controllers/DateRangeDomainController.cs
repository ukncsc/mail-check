using System;
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
    public abstract class DateRangeDomainController : Controller
    {
        private readonly IDomainsDao _domainsDao;
        private readonly IValidator<DateRangeDomainRequest> _dateRangeDomainValidator;
        private readonly ILogger _log;

        protected DateRangeDomainController(IDomainsDao domainsDao,
            IValidator<DateRangeDomainRequest> dateRangeDomainValidator,
            ILogger log)
        {
            _domainsDao = domainsDao;
            _dateRangeDomainValidator = dateRangeDomainValidator;
            _log = log;
        }

        public async Task<IActionResult> GetDateRangeDomainResult<T>(DateRangeDomainRequest dateRangeDomainRequest,
            Func<int, DateTime, DateTime, int?, Task<T>> resultGetter)
        {
            ValidationResult validationResult = await _dateRangeDomainValidator.ValidateAsync(dateRangeDomainRequest);
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

            if (dateRangeDomainRequest.DomainId.HasValue)
            {
                bool domainExists = await _domainsDao.DomainExists(userId, dateRangeDomainRequest.DomainId.Value);
                if (!domainExists)
                {
                    _log.LogWarning($"No domain exists with id for user {userId}: {dateRangeDomainRequest.DomainId.Value}");
                    return NotFound(new ErrorResponse("Domain not found.", ErrorStatus.Information));
                }
            }

            T result = await resultGetter(userId, dateRangeDomainRequest.BeginDateUtc.Value,
                dateRangeDomainRequest.EndDateUtc.Value, dateRangeDomainRequest.DomainId);

            return new ObjectResult(result);
        }

        private int GetUserId(ClaimsPrincipal claimsPrincipal)
        {
            Claim idClaim = claimsPrincipal.FindFirst(_ => _.Type == ClaimTypes.Sid);

            return int.Parse(idClaim.Value);
        }
    }
}
