using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.Runtime;
using Dmarc.Admin.Api.Dao.Search;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Api.Utils;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dmarc.Common.Api.Domain;

namespace Dmarc.Admin.Api.Controllers
{
    [Route("api/admin/search")]
    [Authorize(Policy = PolicyType.Admin)]
    public class SearchController : Controller
    {
        private readonly ISearchDao _searchDao;
        private readonly IValidator<AllEntitiesSearchRequest> _searhLimitRequestValidator;
        private readonly ILogger<SearchController> _log;

        public SearchController(ISearchDao searchDao,
            IValidator<AllEntitiesSearchRequest> searhLimitRequestValidator,
            ILogger<SearchController> log)
        {
            _searchDao = searchDao;
            _searhLimitRequestValidator = searhLimitRequestValidator;
            _log = log;
        }

        [Route("{search}")]
        public async Task<IActionResult> GetSearchResults(AllEntitiesSearchRequest request)
        {
            ValidationResult validationResult = _searhLimitRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            SearchResult searchResult = await _searchDao.GetSearchResults(request.Search, request.Limit);
            return new ObjectResult(searchResult);
        }
    }
}
