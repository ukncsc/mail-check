using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Admin.Api.Dao.Group;
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
    [Route("api/admin/user")]
    public class UserController : Controller
    {
        private readonly IUserDao _userDao;
        private readonly IGroupDao _groupDao;
        private readonly IDomainDao _domainDao;
        private readonly IGroupUserDao _groupUserDao;
        private readonly IValidator<GetEntitiesByRelatedIdRequest> _searchablePagedRequestValidator;
        private readonly IValidator<ChangeMembershipRequest> _idEntityIdsRequestValidator;
        private readonly IValidator<UserForCreation> _userForCreationValidator;
        private readonly IValidator<EntitySearchRequest> _searchLimitExcludedIdsRequestValidator;
        private readonly ILogger<UserController> _log;

        public UserController(IUserDao userDao,
            IGroupDao groupDao,
            IDomainDao domainDao,
            IGroupUserDao groupUserDao,
            IValidator<GetEntitiesByRelatedIdRequest> searchablePagedRequestValidator,
            IValidator<ChangeMembershipRequest> idEntityIdsRequestValidator,
            IValidator<UserForCreation> userForCreationValidator,
            IValidator<EntitySearchRequest> searchLimitExcludedIdsRequestValidator,
            ILogger<UserController> log)
        {
            _userDao = userDao;
            _groupDao = groupDao;
            _domainDao = domainDao;
            _groupUserDao = groupUserDao;
            _searchablePagedRequestValidator = searchablePagedRequestValidator;
            _idEntityIdsRequestValidator = idEntityIdsRequestValidator;
            _userForCreationValidator = userForCreationValidator;
            _searchLimitExcludedIdsRequestValidator = searchLimitExcludedIdsRequestValidator;
            _log = log;
        }

        [Route("current", Name = nameof(GetCurrentUser))]
        public async Task<IActionResult> GetCurrentUser()
        {
            string sid = User.FindFirst(_ => _.Type == ClaimTypes.Sid)?.Value;
            string firstName = User.FindFirst(_ => _.Type == ClaimTypes.Name)?.Value;
            string lastName = User.FindFirst(_ => _.Type == ClaimTypes.Surname)?.Value;
            string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
            string role = User.FindFirst(_ => _.Type == ClaimTypes.Role)?.Value;

            int id;
            int? idValue = int.TryParse(sid, out id) ? id : (int?)null;

            List<DomainPermission> domainPermissions = role == RoleType.Standard
                ? await _userDao.GetDomainPermissions(id)
                : new List<DomainPermission>();

            User user = new User(idValue, firstName, lastName, email, role);
            UserPermissions userPermissions = new UserPermissions(user, domainPermissions);

            return new ObjectResult(userPermissions);
        }

        [Route("{id}", Name = nameof(GetUser))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetUser(int id)
        {
            User user = await _userDao.GetUserById(id);

            return user == null
                ? NotFound(new ErrorResponse("User not found."))
                : new ObjectResult(user);
        }

        [Route("{id}/group", Name = nameof(GetUserGroups))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetUserGroups(GetEntitiesByRelatedIdRequest request)
        {
            ValidationResult validationResult = _searchablePagedRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Group> groups = await _groupDao.GetGroupsByUserId(request.Id, request.Search, request.Page, request.PageSize);
            return new ObjectResult(groups);
        }

        [Route("{id}/domain", Name = nameof(GetUserDomains))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> GetUserDomains(GetEntitiesByRelatedIdRequest request)
        {
            ValidationResult validationResult = _searchablePagedRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Domain.Domain> domains = await _domainDao.GetDomainsByUserId(request.Id, request.Search, request.Page, request.PageSize);
            return new ObjectResult(domains);
        }

        [HttpPatch]
        [Route("{id}/group", Name = nameof(AddGroupsForUser))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> AddGroupsForUser(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupUsers = request.EntityIds.Select(_ => Tuple.Create(_, request.Id)).ToList();
            await _groupUserDao.AddGroupUsers(groupUsers);

            return CreatedAtRoute(nameof(GetUserGroups), new { request.Id }, null);
        }

        [HttpDelete]
        [Route("{id}/group", Name = nameof(DeleteGroupsForUser))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> DeleteGroupsForUser(ChangeMembershipRequest request)
        {
            ValidationResult validationResult = _idEntityIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<Tuple<int, int>> groupUsers = request.EntityIds.Select(_ => Tuple.Create(_, request.Id)).ToList();
            await _groupUserDao.DeleteGroupUsers(groupUsers);

            return new OkObjectResult(new { });
        }

        [HttpPost]
        [Route("", Name = nameof(AddUser))]
        [Authorize(Policy = PolicyType.Admin)]
        public async Task<IActionResult> AddUser([FromBody] UserForCreation user)
        {
            ValidationResult validationResult = _userForCreationValidator.Validate(user);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            User newUser = await _userDao.CreateUser(user);

            return CreatedAtRoute(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        [Authorize(Policy = PolicyType.Admin)]
        [Route("search/{search?}")]
        public async Task<IActionResult> GetUsersSearchResults(EntitySearchRequest request)
        {
            ValidationResult validationResult = _searchLimitExcludedIdsRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                string email = User.FindFirst(_ => _.Type == ClaimTypes.Email)?.Value;
                _log.LogWarning($"User {email} made bad request: {validationResult.GetErrorString()}");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            List<User> users = await _userDao.GetUsersByFirstNameLastNameEmail(request.Search, request.Limit, request.IncludedIds);
            return new ObjectResult(users);
        }
    }
}
