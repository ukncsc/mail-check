using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dmarc.Admin.Api.Contract.Messages;
using Dmarc.Admin.Api.Controllers;
using Dmarc.Admin.Api.Dao.Domain;
using Dmarc.Admin.Api.Dao.Group;
using Dmarc.Admin.Api.Dao.GroupDomain;
using Dmarc.Admin.Api.Dao.User;
using Dmarc.Admin.Api.Domain;
using Dmarc.Common.Interface.Messaging;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Dmarc.Admin.Api.Test.Controllers
{
    public class DomainContollerTests
    {
        private DomainContoller _domainContoller;
        private IUserDao _userDao;
        private IGroupDao _groupDao;
        private IGroupDomainDao _groupDomainDao;
        private IValidator<GetEntitiesByRelatedIdRequest> _idSearchablePagedRequestValidator;
        private IValidator<ChangeMembershipRequest> _idEntityIdsRequestValidator;
        private IValidator<DomainForCreation> _domainForCreationValidator;
        private IValidator<EntitySearchRequest> _searchLimitExcludedIdsRequestValidator;
        private IValidator<PublicDomainForCreation> _publicDomainForCreationValidator;
        private ILogger<DomainContoller> _log;
        private IDomainDao _domainDao;
        private IPublisher _publisher;
        private IPublisherConfig _publisherConfig;

        [SetUp]
        public void SetUp()
        {
            _domainDao = A.Fake<IDomainDao>();
            _userDao = A.Fake<IUserDao>();
            _groupDao = A.Fake<IGroupDao>();
            _groupDomainDao = A.Fake<IGroupDomainDao>();
            _idSearchablePagedRequestValidator = A.Fake<IValidator<GetEntitiesByRelatedIdRequest>>();
            _idEntityIdsRequestValidator = A.Fake<IValidator<ChangeMembershipRequest>>();
            _domainForCreationValidator = A.Fake<IValidator<DomainForCreation>>();
            _searchLimitExcludedIdsRequestValidator = A.Fake<IValidator<EntitySearchRequest>>();
            _publicDomainForCreationValidator = A.Fake<IValidator<PublicDomainForCreation>>();
            _log = A.Fake<ILogger<DomainContoller>>();
            _publisher = A.Fake<IPublisher>();
            _publisherConfig = A.Fake<IPublisherConfig>();

            _domainContoller = new DomainContoller(_domainDao, _userDao, _groupDao, _groupDomainDao,
                _idSearchablePagedRequestValidator, _idEntityIdsRequestValidator, _domainForCreationValidator,
                _searchLimitExcludedIdsRequestValidator,
                _publicDomainForCreationValidator, _log,  _publisher, _publisherConfig)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };
        }

        [Test]
        public async Task InvalidDomainBadRequestResponse()
        {
            SetSid("1", "james@abc.gov.uk", _domainContoller);
            
            DomainForCreation request = new DomainForCreation { Name = "abcgovuk" };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure(string.Empty, string.Empty) });
            A.CallTo(() => _domainForCreationValidator.Validate(request)).Returns(validationResult);
            IActionResult result = await _domainContoller.AddDomain(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            A.CallTo(() => _domainDao.CreateDomain(A<string>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _publisher.Publish(A<string>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _publisherConfig.PublisherConnectionString).MustNotHaveHappened();
        }

        [Test]
        public async Task StandardUserAddingNonPublicDomainReturnsBadRequestResponse()
        {
            SetSid("1", "james@abc.gov.uk", _domainContoller, "Standard");

            var errorValidationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure(string.Empty, string.Empty) });

            DomainForCreation request = new DomainForCreation { Name = "abc.com" };
            A.CallTo(() => _domainForCreationValidator.Validate(request)).Returns(new ValidationResult());
            A.CallTo(() => _publicDomainForCreationValidator.Validate(request)).Returns(errorValidationResult);
            IActionResult result = await _domainContoller.AddDomain(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            A.CallTo(() => _domainDao.CreateDomain(A<string>._, A<int>._)).MustNotHaveHappened();
            A.CallTo(() => _publisher.Publish(A<string>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _publisherConfig.PublisherConnectionString).MustNotHaveHappened();
        }

        [Test]
        public async Task StandardUserAddingPublicDomain()
        {
            SetSid("1", "james@abc.gov.uk", _domainContoller, "Standard");
            
            DomainForCreation request = new DomainForCreation { Name = "abc.gov.uk" };
            A.CallTo(() => _domainForCreationValidator.Validate(request)).Returns(new ValidationResult());
            A.CallTo(() => _publicDomainForCreationValidator.Validate(A< PublicDomainForCreation>._)).Returns(new ValidationResult());
            IActionResult result = await _domainContoller.AddDomain(request);

            Assert.That(result, Is.TypeOf<CreatedAtRouteResult>());
            A.CallTo(() => _domainDao.CreateDomain(A<string>._, A<int>._)).MustHaveHappened();
            A.CallTo(() => _publisher.Publish(A<DomainCreated>._, A<string>._)).MustHaveHappened();
            A.CallTo(() => _publisherConfig.PublisherConnectionString).MustHaveHappened();
        }

        [Test]
        public async Task AdminUserAddingPublicDomain()
        {
            SetSid("1", "james@abc.gov.uk", _domainContoller);

            DomainForCreation request = new DomainForCreation { Name = "abc.gov.uk" };
            A.CallTo(() => _domainForCreationValidator.Validate(request)).Returns(new ValidationResult());
            A.CallTo(() => _publicDomainForCreationValidator.Validate(A<PublicDomainForCreation>._)).Returns(new ValidationResult());
            IActionResult result = await _domainContoller.AddDomain(request);

            Assert.That(result, Is.TypeOf<CreatedAtRouteResult>());
            A.CallTo(() => _domainDao.CreateDomain(A<string>._, A<int>._)).MustHaveHappened();
            A.CallTo(() => _publisher.Publish(A<DomainCreated>._, A<string>._)).MustHaveHappened();
            A.CallTo(() => _publisherConfig.PublisherConnectionString).MustHaveHappened();
        }

        [Test]
        public async Task AdminUserAddingNonPublicDomain()
        {
            SetSid("1", "james@abc.gov.uk", _domainContoller);

            DomainForCreation request = new DomainForCreation { Name = "abc.com" };
            A.CallTo(() => _domainForCreationValidator.Validate(request)).Returns(new ValidationResult());
            A.CallTo(() => _publicDomainForCreationValidator.Validate(A<PublicDomainForCreation>._)).Returns(new ValidationResult());
            IActionResult result = await _domainContoller.AddDomain(request);

            Assert.That(result, Is.TypeOf<CreatedAtRouteResult>());
            A.CallTo(() => _domainDao.CreateDomain(A<string>._, A<int>._)).MustHaveHappened();
            A.CallTo(() => _publisherConfig.PublisherConnectionString).MustHaveHappened();
            A.CallTo(() => _publisher.Publish(A<DomainCreated>._, A<string>._)).MustHaveHappened();
        }

        private void SetSid(string sid,  string email, Controller controller, string role = "Admin")
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Sid, sid),
                        new Claim(ClaimTypes.Role, role),
                        new Claim(ClaimTypes.Email, email)
                    }, "AuthTypeName"))
                }
            };
        }
    }
}
