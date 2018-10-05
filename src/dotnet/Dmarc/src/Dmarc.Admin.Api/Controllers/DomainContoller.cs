using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Contract.Messages;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Admin.Api.Dao.Group;
using Dmarc.Admin.Api.Dao.GroupDomain;
using Dmarc.Admin.Api.Dao.User;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Api.Identity.Domain;
using Dmarc.Common.Api.Utils;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Dmarc.Common.Api.Domain;
using Dmarc.Common.Interface.Messaging;

namespace Dmarc.Admin.Api.Controllers
{
    [Route("api/admin/domain")]
    public class DomainContoller : Controller
    {
        private readonly IDomainDao _domainDao;
        private readonly IUserDao _userDao;
        private readonly IGroupDao _groupDao;
        private readonly IGroupDomainDao _groupDomainDao;
        private readonly IValidator<GetEntitiesByRelatedIdRequest> _idSearchablePagedRequestValidator;
        private readonly IValidator<ChangeMembershipRequest> _idEntityIdsRequestValidator;
        private readonly IValidator<DomainForCreation> _domainForCreationValidator;
        private readonly IValidator<EntitySearchRequest> _searchLimitExcludedIdsRequestValidator;
        private readonly IValidator<PublicDomainForCreation> _publicDomainForCreationValidator;
        private readonly ILogger<DomainContoller> _log;
        private readonly IPublisher _publisher;
        private readonly IPublisherConfig _config;

        public DomainContoller(IDomainDao domainDao,
            IUserDao userDao,
            IGroupDao groupDao,
            IGroupDomainDao groupDomainDao,
            IValidator<GetEntitiesByRelatedIdRequest> idSearchablePagedRequestValidator,
            IValidator<ChangeMembershipRequest> idEntityIdsRequestValidator,
            IValidator<DomainForCreation> domainForCreationValidator,
            IValidator<EntitySearchRequest> searchLimitExcludedIdsRequestValidator,
            IValidator<PublicDomainForCreation> publicDomainForCreationValidator,
            ILogger<DomainContoller> log,
            IPublisher publisher,
            IPublisherConfig config)
        {
            _domainDao = domainDao;
            _userDao = userDao;
            _groupDao = groupDao;
            _groupDomainDao = groupDomainDao;
            _idSearchablePagedRequestValidator = idSearchablePagedRequestValidator;
            _idEntityIdsRequestValidator = idEntityIdsRequestValidator;
            _domainForCreationValidator = domainForCreationValidator;
            _searchLimitExcludedIdsRequestValidator = searchLimitExcludedIdsRequestValidator;
            _publicDomainForCreationValidator = publicDomainForCreationValidator;
            _log = log;
            _publisher = publisher;
            _config = config;
        }

        [Route("{id}", Name = nameof(GetDomain))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetDomain(int id)
        {
            Domain.Domain domain = await _domainDao.GetDomainById(id);
            return domain == null
                ? NotFound(new ErrorResponse("Domain not found.", ErrorStatus.Information))
                : new ObjectResult(domain);
        }

        [Route("{id}/user", Name = nameof(GetDomainUsers))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetDomainUsers(GetEntitiesByRelatedIdRequest request)
        {
            ValidationResult validationResult = _idSearchablePagedRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.GetEmail();
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<User> users =
                await _userDao.GetUsersByDomainId(request.Id, request.Search, request.Page, request.PageSize);

            return new ObjectResult(users);
        }

        [Route("{id}/group", Name = nameof(GetDomainGroups))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetDomainGroups(GetEntitiesByRelatedIdRequest request)
        {
            ValidationResult validationResult = _idSearchablePagedRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.GetEmail();
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Group> groups =
                await _groupDao.GetGroupsByDomainId(request.Id, request.Search, request.Page, request.PageSize);

            return new ObjectResult(groups);
        }

        [HttpPatch]
        [Route("{id}/group", Name = nameof(AddGroupsForDomain))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> AddGroupsForDomain(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.GetEmail();
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupDomains = request.EntityIds.Select(_ => Tuple.Create(_, request.Id)).ToList();
            await _groupDomainDao.AddGroupDomains(groupDomains);

            return CreatedAtRoute(nameof(GetDomainGroups), new {request.Id}, null);
        }

        [HttpDelete]
        [Route("{id}/group", Name = nameof(DeleteGroupsForDomain))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> DeleteGroupsForDomain(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.GetEmail();
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupDomains = request.EntityIds.Select(_ => Tuple.Create(_, request.Id)).ToList();
            await _groupDomainDao.DeleteGroupDomains(groupDomains);

            return new OkObjectResult(new { });
        }

        [HttpPost]
        [Route("", Name = nameof(AddDomain))]
        [Authorize(Policy = PolicyType.Standard)]
        public async Task<IActionResult> AddDomain([FromBody] DomainForCreation domain)
        {
            string email = User.GetEmail();

            ValidationResult validationResult = _domainForCreationValidator.Validate(domain);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            if (!User.IsAdmin())
            {
                ValidationResult publicDomainValidationResult =
                    _publicDomainForCreationValidator.Validate(new PublicDomainForCreation {Name = domain.Name});
                if (!publicDomainValidationResult.IsValid)
                {
                    _log.LogWarning($"User {email} made bad request: {publicDomainValidationResult.GetErrorString()}");
                    return BadRequest(new ErrorResponse(publicDomainValidationResult.GetErrorString()));
                }
            }
            
            int? userId = User.GetId();

            if (!userId.HasValue)
            {
                return BadRequest(new ErrorResponse("Unable to retrieve user id"));
            }

            Domain.Domain newDomain = await _domainDao.CreateDomain(domain.Name, userId.Value);

            await _publisher.Publish(new DomainCreated(domain.Name, email, DateTime.UtcNow),
                _config.PublisherConnectionString);

            return CreatedAtRoute(nameof(GetDomain), new {id = newDomain.Id}, newDomain);
        }

        [Route("search/{search?}")]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetDomainsSearchResults(EntitySearchRequest request)
        {
            ValidationResult validationResult = _searchLimitExcludedIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.GetEmail();
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Domain.Domain> domains = await _domainDao.GetDomainsByName(request.Search, request.Limit, request.IncludedIds);
            return new ObjectResult(domains);
        }
    }
}
