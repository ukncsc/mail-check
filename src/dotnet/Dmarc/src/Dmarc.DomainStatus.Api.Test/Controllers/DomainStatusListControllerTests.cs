using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DomainStatus.Api.Controllers;
using Dmarc.DomainStatus.Api.Dao.DomainStatusList;
using Dmarc.DomainStatus.Api.Domain;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Dmarc.DomainStatus.Api.Services;
using Dmarc.Common.Validation;

namespace Dmarc.DomainStatus.Api.Test.Controllers
{
    [TestFixture]
    public class DomainStatusListControllerTests
    {
        private const int DomainId1 = 1;
        private const int DomainId2 = 2;
        private const int DomainId3 = 3;

        private const string SubDomain1 = "abc.xyz.com";
        private const string SubDomain2 = "def.xyz.com";
        private const string OrgDomain1 = "xyz.com";

        private const Status Success = Status.Success;
        private const Status Warning = Status.Warning;
        private const Status Error = Status.Error;

        private DomainStatusListController _domainStatusListController;
        private IDomainStatusListDao _domainStatusListDao;
        private ICertificateEvaluatorApi _certificateEvaluatorApi;
        private IDomainValidator _domainValidator;
        private IPublicDomainListValidator _publicDomainValidator;
        private IOrganisationalDomainProvider _organisationDomainProvider;
        private IValidator<DomainsRequest> _domainRequestValidator;

        [SetUp]
        public void SetUp()
        {
            _domainStatusListDao = A.Fake<IDomainStatusListDao>();
            _organisationDomainProvider = A.Fake<IOrganisationalDomainProvider>();
            _domainRequestValidator = A.Fake<IValidator<DomainsRequest>>();
            _certificateEvaluatorApi = A.Fake<ICertificateEvaluatorApi>();
            _domainValidator = A.Fake<IDomainValidator>();
            _publicDomainValidator = A.Fake<IPublicDomainListValidator>();

            _domainStatusListController = new DomainStatusListController(_domainStatusListDao, _organisationDomainProvider,
                _certificateEvaluatorApi, _domainValidator, _publicDomainValidator, _domainRequestValidator, A.Fake<ILogger<DomainStatusListController>>());

            _domainStatusListController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
        }

        [Test]
        public async Task GetDomainSecurityInfoInvalidRequestReturnsBadRequest()
        {
            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure(string.Empty, string.Empty) });

            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(validationResult));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfo(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsCount(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDomainSecurityInfoAllRecordsHaveDmarcReturnsResult()
        {
            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            var domainSecurityInfos = new List<DomainSecurityInfo>()
            {
                CreateDomainSecurityInfo(DomainId1, SubDomain1, true, Success),
                CreateDomainSecurityInfo(DomainId2, SubDomain2, true, Warning)
            };

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).Returns(domainSecurityInfos);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).Returns(domainSecurityInfos);

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfo(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<DomainsSecurityResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;

            Assert.That(response.DomainSecurityInfos[0].Domain.Id, Is.EqualTo(DomainId1));
            Assert.That(response.DomainSecurityInfos[0].Domain.Name, Is.EqualTo(SubDomain1));
            Assert.That(response.DomainSecurityInfos[0].DmarcStatus, Is.EqualTo(Success));

            Assert.That(response.DomainSecurityInfos[1].Domain.Id, Is.EqualTo(DomainId2));
            Assert.That(response.DomainSecurityInfos[1].Domain.Name, Is.EqualTo(SubDomain2));
            Assert.That(response.DomainSecurityInfos[1].DmarcStatus, Is.EqualTo(Warning));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsCount(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDomainSecurityInfoForDomainsWhenDomainAndOrgDomainsDontHaveDmarcReturnsOrginalRecord()
        {
            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            var domainSecurityInfos = new List<DomainSecurityInfo>()
            {
                CreateDomainSecurityInfo(DomainId1, SubDomain1, false, Error),
                CreateDomainSecurityInfo(DomainId2, SubDomain2, true, Warning)
            };

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).Returns(domainSecurityInfos);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).Returns(domainSecurityInfos);

            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(SubDomain1))
                .Returns(Task.FromResult(new OrganisationalDomain(OrgDomain1, SubDomain1)));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).Returns(
                Task.FromResult(new List<DomainSecurityInfo>
                {
                    CreateDomainSecurityInfo(DomainId3, OrgDomain1, false, Warning),
                }));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfo(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<DomainsSecurityResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;

            Assert.That(response.DomainSecurityInfos[0].Domain.Id, Is.EqualTo(DomainId1));
            Assert.That(response.DomainSecurityInfos[0].Domain.Name, Is.EqualTo(SubDomain1));
            Assert.That(response.DomainSecurityInfos[0].DmarcStatus, Is.EqualTo(Error));

            Assert.That(response.DomainSecurityInfos[1].Domain.Id, Is.EqualTo(DomainId2));
            Assert.That(response.DomainSecurityInfos[1].Domain.Name, Is.EqualTo(SubDomain2));
            Assert.That(response.DomainSecurityInfos[1].DmarcStatus, Is.EqualTo(Warning));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsCount(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task GetDomainSecurityInfoWhenDomainDoesntHaveDmarcButOrgDomainDoesReturnsOrgDomainResult()
        {
            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            var domainSecurityInfos = new List<DomainSecurityInfo>()
            {
                CreateDomainSecurityInfo(DomainId1, SubDomain1, false, Error),
                CreateDomainSecurityInfo(DomainId2, SubDomain2, true, Warning)
            };

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).Returns(domainSecurityInfos);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).Returns(domainSecurityInfos);

            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(SubDomain1))
                .Returns(Task.FromResult(new OrganisationalDomain(OrgDomain1, SubDomain1)));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).Returns(
                Task.FromResult(new List<DomainSecurityInfo>
                {
                    CreateDomainSecurityInfo(DomainId3, OrgDomain1, true, Success),
                }));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfo(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<DomainsSecurityResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;

            Assert.That(response.DomainSecurityInfos[0].Domain.Id, Is.EqualTo(DomainId1));
            Assert.That(response.DomainSecurityInfos[0].Domain.Name, Is.EqualTo(SubDomain1));
            Assert.That(response.DomainSecurityInfos[0].DmarcStatus, Is.EqualTo(Success));

            Assert.That(response.DomainSecurityInfos[1].Domain.Id, Is.EqualTo(DomainId2));
            Assert.That(response.DomainSecurityInfos[1].Domain.Name, Is.EqualTo(SubDomain2));
            Assert.That(response.DomainSecurityInfos[1].DmarcStatus, Is.EqualTo(Warning));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfo(A<int>._, A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsCount(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task GetDomainSecurityInfoByUserIdInvalidRequestReturnsBadRequest()
        {
            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure(string.Empty, string.Empty) });

            SetSid("1", _domainStatusListController);

            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(validationResult));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfoByUserId(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustNotHaveHappened();

            A.CallTo(() => _domainStatusListDao.GetDomainsCountByUserId(A<int>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDomainSecurityInfoByUserIdNoUserIdEmptyResult()
        {
            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                 .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfoByUserId(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<MyDomainsResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;
            Assert.That(response.DomainSecurityInfos, Is.Empty);

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsCountByUserId(A<int>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDomainSecurityInfoByUserIdAllRecordsHaveDmarcReturnsResult()
        {
            SetSid("1", _domainStatusListController);

            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            var domainSecurityInfos = new List<DomainSecurityInfo>()
            {
                CreateDomainSecurityInfo(DomainId1, SubDomain1, true, Success),
                CreateDomainSecurityInfo(DomainId2, SubDomain2, true, Warning)
            };

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).Returns(domainSecurityInfos);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).Returns(domainSecurityInfos);

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfoByUserId(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<MyDomainsResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;

            Assert.That(response.DomainSecurityInfos[0].Domain.Id, Is.EqualTo(DomainId1));
            Assert.That(response.DomainSecurityInfos[0].Domain.Name, Is.EqualTo(SubDomain1));
            Assert.That(response.DomainSecurityInfos[0].DmarcStatus, Is.EqualTo(Success));

            Assert.That(response.DomainSecurityInfos[1].Domain.Id, Is.EqualTo(DomainId2));
            Assert.That(response.DomainSecurityInfos[1].Domain.Name, Is.EqualTo(SubDomain2));
            Assert.That(response.DomainSecurityInfos[1].DmarcStatus, Is.EqualTo(Warning));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsCountByUserId(A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDomainSecurityInfoByUserIdForDomainsWhenDomainAndOrgDomainsDontHaveDmarcReturnsOrginalRecord()
        {
            SetSid("1", _domainStatusListController);

            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            var domainSecurityInfos = new List<DomainSecurityInfo>()
            {
                CreateDomainSecurityInfo(DomainId1, SubDomain1, false, Error),
                CreateDomainSecurityInfo(DomainId2, SubDomain2, true, Warning)
            };

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).Returns(domainSecurityInfos);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).Returns(domainSecurityInfos);

            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(SubDomain1))
                .Returns(Task.FromResult(new OrganisationalDomain(OrgDomain1, SubDomain1)));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).Returns(
                Task.FromResult(new List<DomainSecurityInfo>
                {
                    CreateDomainSecurityInfo(DomainId3, OrgDomain1, false, Warning),
                }));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfoByUserId(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<MyDomainsResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;

            Assert.That(response.DomainSecurityInfos[0].Domain.Id, Is.EqualTo(DomainId1));
            Assert.That(response.DomainSecurityInfos[0].Domain.Name, Is.EqualTo(SubDomain1));
            Assert.That(response.DomainSecurityInfos[0].DmarcStatus, Is.EqualTo(Error));

            Assert.That(response.DomainSecurityInfos[1].Domain.Id, Is.EqualTo(DomainId2));
            Assert.That(response.DomainSecurityInfos[1].Domain.Name, Is.EqualTo(SubDomain2));
            Assert.That(response.DomainSecurityInfos[1].DmarcStatus, Is.EqualTo(Warning));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsCountByUserId(A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task GetDomainSecurityInfoByUserIdWhenDomainDoesntHaveDmarcButOrgDomainDoesReturnsOrgDomainResult()
        {
            SetSid("1", _domainStatusListController);

            A.CallTo(() => _domainRequestValidator.ValidateAsync(A<DomainsRequest>._, CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult(new List<ValidationFailure>())));

            var domainSecurityInfos = new List<DomainSecurityInfo>()
            {
                CreateDomainSecurityInfo(DomainId1, SubDomain1, false, Error),
                CreateDomainSecurityInfo(DomainId2, SubDomain2, true, Warning)
            };

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).Returns(domainSecurityInfos);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).Returns(domainSecurityInfos);

            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(SubDomain1))
                .Returns(Task.FromResult(new OrganisationalDomain(OrgDomain1, SubDomain1)));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).Returns(
                Task.FromResult(new List<DomainSecurityInfo>
                {
                    CreateDomainSecurityInfo(DomainId3, OrgDomain1, true, Success),
                }));

            DomainsRequest request = new DomainsRequest();
            IActionResult result = await _domainStatusListController.GetDomainsSecurityInfoByUserId(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<MyDomainsResponse>());

            DomainsSecurityResponse response = objectResult.Value as DomainsSecurityResponse;

            Assert.That(response.DomainSecurityInfos[0].Domain.Id, Is.EqualTo(DomainId1));
            Assert.That(response.DomainSecurityInfos[0].Domain.Name, Is.EqualTo(SubDomain1));
            Assert.That(response.DomainSecurityInfos[0].DmarcStatus, Is.EqualTo(Success));

            Assert.That(response.DomainSecurityInfos[1].Domain.Id, Is.EqualTo(DomainId2));
            Assert.That(response.DomainSecurityInfos[1].Domain.Name, Is.EqualTo(SubDomain2));
            Assert.That(response.DomainSecurityInfos[1].DmarcStatus, Is.EqualTo(Warning));

            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByUserId(A<int>._, A<int>._, A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _certificateEvaluatorApi.UpdateTlsWithCertificateEvaluatorStatus(A<List<DomainSecurityInfo>>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsCountByUserId(A<int>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => _organisationDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusListDao.GetDomainsSecurityInfoByDomainNames(A<List<string>>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        #region TestSupport

        private DomainSecurityInfo CreateDomainSecurityInfo(int domainId, string domainName, bool hasDmarc, Status dmarcStatus)
        {
            Domain.Domain domain = new Domain.Domain(domainId, domainName);
            return new DomainSecurityInfo(domain, hasDmarc, Status.Success, dmarcStatus, Status.Success);
        }

        private void SetSid(string sid, Controller controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Sid, sid)
                        }, "AuthTypeName"))
                }
            };
        }

        #endregion TestSupport
    }
}
