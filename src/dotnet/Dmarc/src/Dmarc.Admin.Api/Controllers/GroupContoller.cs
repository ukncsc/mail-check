using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Admin.Api.Dao.Group;
using Dmarc.Admin.Api.Dao.GroupDomain;
using Dmarc.Admin.Api.Dao.GroupUser;
using Dmarc.Admin.Api.Dao.User;
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
    [Route("api/admin/group")]
    [Authorize(Policy = PolicyType.Admin)]
    public class GroupContoller : Controller
    {
        private readonly IGroupDao _groupDao;
        private readonly IUserDao _userDao;
        private readonly IDomainDao _domainDao;
        private readonly IGroupUserDao _groupUserDao;
        private readonly IGroupDomainDao _groupDomainDao;
        private readonly IValidator<GetEntitiesByRelatedIdRequest> _searchablePagedRequestValidator;
        private readonly IValidator<ChangeMembershipRequest> _idEntityIdsRequestValidator;
        private readonly IValidator<EntitySearchRequest> _searchLimitExcludedIdsRequestValidator;
        private readonly IValidator<GroupForCreation> _groupsForCreationValidator;
        private readonly ILogger<GroupContoller> _log;

        public GroupContoller(IGroupDao groupDao,
            IUserDao userDao,
            IDomainDao domainDao,
            IGroupUserDao groupUserDao,
            IGroupDomainDao groupDomainDao,
            IValidator<GetEntitiesByRelatedIdRequest> searchablePagedRequestValidator,
            IValidator<ChangeMembershipRequest> idEntityIdsRequestValidator,
            IValidator<EntitySearchRequest> searchLimitExcludedIdsRequestValidator,
            ILogger<GroupContoller> log, IValidator<GroupForCreation> groupsForCreationValidator)
        {
            _groupDao = groupDao;
            _userDao = userDao;
            _domainDao = domainDao;
            _groupUserDao = groupUserDao;
            _groupDomainDao = groupDomainDao;
            _searchablePagedRequestValidator = searchablePagedRequestValidator;
            _idEntityIdsRequestValidator = idEntityIdsRequestValidator;
            _searchLimitExcludedIdsRequestValidator = searchLimitExcludedIdsRequestValidator;
            _log = log;
            _groupsForCreationValidator = groupsForCreationValidator;
        }

        [Route("{id}", Name = nameof(GetGroup))]
        public async Task<IActionResult> GetGroup(int id)
        {
            Group group = await _groupDao.GetGroupById(id);
            return group == null
                ? NotFound(new ErrorResponse("Group not found.", ErrorStatus.Information))
                : new ObjectResult(group);
        }

        [Route("{id}/user", Name = nameof(GetGroupUsers))]
        public async Task<IActionResult> GetGroupUsers(GetEntitiesByRelatedIdRequest request)
        {
            ValidationResult validationResult = _searchablePagedRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<User> users = await _userDao.GetUsersByGroupId(request.Id, request.Search, request.Page, request.PageSize);
            return new ObjectResult(users);
        }

        [Route("{id}/domain", Name = nameof(GetGroupDomains))]
        public async Task<IActionResult> GetGroupDomains(GetEntitiesByRelatedIdRequest request)
        {
            ValidationResult validationResult = _searchablePagedRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Domain.Domain> domains = await _domainDao.GetDomainsByGroupId(request.Id, request.Search, request.Page, request.PageSize);
            return new ObjectResult(domains);
        }

        [HttpPatch]
        [Route("{id}/user", Name = nameof(AddUsersToGroup))]
        public async Task<IActionResult> AddUsersToGroup(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupUsers = request.EntityIds.Select(_ => Tuple.Create(request.Id, _)).ToList();
            await _groupUserDao.AddGroupUsers(groupUsers);

            return CreatedAtRoute(nameof(GetGroupUsers), new { request.Id }, null);
        }

        [HttpDelete]
        [Route("{id}/user", Name = nameof(DeleteUsersFromGroup))]
        public async Task<IActionResult> DeleteUsersFromGroup(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupUsers = request.EntityIds.Select(_ => Tuple.Create(request.Id, _)).ToList();
            await _groupUserDao.DeleteGroupUsers(groupUsers);

            return new OkObjectResult(new { });
        }

        [HttpPatch]
        [Route("{id}/domain", Name = nameof(AddDomainsToGroup))]
        public async Task<IActionResult> AddDomainsToGroup(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupDomains = request.EntityIds.Select(_ => Tuple.Create(request.Id, _)).ToList();
            await _groupDomainDao.AddGroupDomains(groupDomains);

            return CreatedAtRoute(nameof(GetGroupDomains), new { request.Id }, null);
        }

        [HttpDelete]
        [Route("{id}/domain", Name = nameof(DeleteDomainsFromGroup))]
        public async Task<IActionResult> DeleteDomainsFromGroup(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupDomains = request.EntityIds.Select(_ => Tuple.Create(request.Id, _)).ToList();
            await _groupDomainDao.DeleteGroupDomains(groupDomains);

            return new OkObjectResult(new { });
        }

        [HttpPost]
        [Route("", Name = nameof(AddGroup))]
        public async Task<IActionResult> AddGroup([FromBody]GroupForCreation group)
        {
            ValidationResult validationResult = _groupsForCreationValidator.Validate(group);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            Group newGroup = await _groupDao.CreateGroup(group);

            return CreatedAtRoute(nameof(GetGroup), new { id = newGroup.Id }, newGroup);
        }

        [Route("search/{search?}")]
        public async Task<IActionResult> GetGroupsSearchResults(EntitySearchRequest request)
        {
            ValidationResult validationResult = _searchLimitExcludedIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Group> groups = await _groupDao.GetGroupsByName(request.Search, request.Limit, request.IncludedIds);
            return new ObjectResult(groups);
        }
    }
}
