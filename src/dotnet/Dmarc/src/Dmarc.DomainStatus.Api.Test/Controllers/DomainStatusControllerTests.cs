using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DomainStatus.Api.Controllers;
using Dmarc.DomainStatus.Api.Dao;
using Dmarc.DomainStatus.Api.Dao.DomainStatus;
using Dmarc.DomainStatus.Api.Dao.Permission;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Util;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Dmarc.DomainStatus.Api.Services;

namespace Dmarc.DomainStatus.Api.Test.Controllers
{
    [TestFixture]
    public class DomainStatusControllerTests
    {
        private DomainStatusController _domainStatusController;
        private IDomainStatusDao _domainStatusDao;
        private IPermissionDao _permissionDao;
        private IValidator<DomainRequest> _domainRequestValidator;
        private IValidator<DomainsRequest> _domainsRequestValidator;
        private IValidator<DateRangeDomainRequest> _dateRangeDomainRequestValidator;
        private IOrganisationalDomainProvider _organisationalDomainProvider;
        private IReverseDnsApi _reverseDnsApi;

        [SetUp]
        public void SetUp()
        {
            _domainStatusDao = A.Fake<IDomainStatusDao>();
            _permissionDao = A.Fake<IPermissionDao>();
            _domainRequestValidator = A.Fake<IValidator<DomainRequest>>();
            _domainsRequestValidator = A.Fake<IValidator<DomainsRequest>>();
            _dateRangeDomainRequestValidator = A.Fake<IValidator<DateRangeDomainRequest>>();
            _organisationalDomainProvider = A.Fake<IOrganisationalDomainProvider>();
            _reverseDnsApi = A.Fake<IReverseDnsApi>();
            _domainStatusController = new DomainStatusController(_domainStatusDao, _permissionDao,
                _organisationalDomainProvider, _reverseDnsApi, _domainRequestValidator, _domainsRequestValidator,
                _dateRangeDomainRequestValidator, A.Fake<ILogger<DomainStatusController>>());

            _domainStatusController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
        }

        [Test]
        public async Task GetAggregateReportSummaryReturnsBadRequestWhenRequestIsInvalid()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure(string.Empty, string.Empty) });
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetAggregateReportSummaryReturnsForbidWhenUnknownUser()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task GetAggregateReportSummaryReturnsNotFoundWhenUserCantViewDomain()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, false, false)));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task GetAggregateReportSummaryReturnsForbidWhenUserCantViewAggregateReportForDomain()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, false, true)));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task GetAggregateReportSummaryReturnsNullWhenNoResults()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, true, true)));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.Null);
        }

        [Test]
        public async Task GetAggregateReportSummaryFillsInHolesInDataset()
        {
            DateTime now = DateTime.Now.Date;
            DateTime dayOne = now.AddDays(-3);
            DateTime dayFour = now;

            DateRangeDomainRequest request = new DateRangeDomainRequest { Id = 1, StartDate = dayOne, EndDate = dayFour };

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, true, true)));

            A.CallTo(() => _domainStatusDao.GetAggregateReportSummary(A<int>._, A<DateTime>._, A<DateTime>._))
                .Returns(Task.FromResult(new SortedDictionary<DateTime, AggregateSummaryItem>
                {
                    {dayOne, new AggregateSummaryItem(1, 1, 1, 1, 1)},
                    {dayFour, new AggregateSummaryItem(4, 4, 4, 4, 4)}
                }));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<AggregateSummary>());

            AggregateSummary aggregateSummary = objectResult.Value as AggregateSummary;
            Assert.That(aggregateSummary.Results.Count, Is.EqualTo(4));

            Assert.That(aggregateSummary.Results.Keys.ElementAt(0), Is.EqualTo(dayOne));
            Assert.That(aggregateSummary.Results.Keys.ElementAt(1), Is.EqualTo(dayOne.AddDays(1)));
            Assert.That(aggregateSummary.Results.Keys.ElementAt(2), Is.EqualTo(dayOne.AddDays(2)));
            Assert.That(aggregateSummary.Results.Keys.ElementAt(3), Is.EqualTo(dayFour));

            Assert.That(aggregateSummary.Results.Values.ElementAt(0).FullyTrusted, Is.EqualTo(1));
            Assert.That(aggregateSummary.Results.Values.ElementAt(1).FullyTrusted, Is.EqualTo(0));
            Assert.That(aggregateSummary.Results.Values.ElementAt(2).FullyTrusted, Is.EqualTo(0));
            Assert.That(aggregateSummary.Results.Values.ElementAt(3).FullyTrusted, Is.EqualTo(4));
        }

        [Test]
        public async Task GetAggregateReportSummaryReturnsResults()
        {
            DateTime now = DateTime.Now.Date;
            DateTime dayOne = now.AddDays(-3);
            DateTime dayFour = now;

            DateRangeDomainRequest request = new DateRangeDomainRequest { Id = 1, StartDate = dayOne, EndDate = dayFour };

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, true, true)));

            A.CallTo(() => _domainStatusDao.GetAggregateReportSummary(A<int>._, A<DateTime>._, A<DateTime>._))
                .Returns(Task.FromResult(new SortedDictionary<DateTime, AggregateSummaryItem>
                {
                    {dayOne, new AggregateSummaryItem(1, 1, 1, 1, 1)},
                    {dayOne.AddDays(1), new AggregateSummaryItem(2, 2, 2, 2, 2)},
                    {dayOne.AddDays(2), new AggregateSummaryItem(3, 3, 3, 3, 3)},
                    {dayFour, new AggregateSummaryItem(4, 4, 4, 4, 4)}
                }));

            IActionResult result = await _domainStatusController.GetAggregateReportSummary(request);

            Assert.That(result, Is.TypeOf<ObjectResult>());

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<AggregateSummary>());

            AggregateSummary aggregateSummary = objectResult.Value as AggregateSummary;
            Assert.That(aggregateSummary.Results.Count, Is.EqualTo(4));

            Assert.That(aggregateSummary.Results.Keys.ElementAt(0), Is.EqualTo(dayOne));
            Assert.That(aggregateSummary.Results.Keys.ElementAt(1), Is.EqualTo(dayOne.AddDays(1)));
            Assert.That(aggregateSummary.Results.Keys.ElementAt(2), Is.EqualTo(dayOne.AddDays(2)));
            Assert.That(aggregateSummary.Results.Keys.ElementAt(3), Is.EqualTo(dayFour));

            Assert.That(aggregateSummary.Results.Values.ElementAt(0).FullyTrusted, Is.EqualTo(1));
            Assert.That(aggregateSummary.Results.Values.ElementAt(1).FullyTrusted, Is.EqualTo(2));
            Assert.That(aggregateSummary.Results.Values.ElementAt(2).FullyTrusted, Is.EqualTo(3));
            Assert.That(aggregateSummary.Results.Values.ElementAt(3).FullyTrusted, Is.EqualTo(4));
        }

        [Test]
        public async Task GetAggregateReportExportReturnsForbidWhenUnknownUser()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            IActionResult result = await _domainStatusController.GetAggregateReportExport(1, DateTime.Now);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task GetAggregateReportExportReturnsNotFoundWhenUserCantViewDomain()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, false, false)));

            IActionResult result = await _domainStatusController.GetAggregateReportExport(1, DateTime.Now);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task GetAggregateReportExportReturnsForbidWhenUserCantViewAggregateReportForDomain()
        {
            DateRangeDomainRequest request = new DateRangeDomainRequest();

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _dateRangeDomainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            SetSid("1", _domainStatusController);

            A.CallTo(() => _permissionDao.GetPermissions(A<int>._, A<int>._)).Returns(Task.FromResult(new DomainPermissions(1, false, true)));

            IActionResult result = await _domainStatusController.GetAggregateReportExport(1, DateTime.Now);

            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task GetDmarcReadModelDomainHasDmarcRecordDmarcRecordReturned()
        {
            int domainId = 1;
            string readmodel = "{\"readModel\": \"Test\"}";
            string domainName = "abc.xyz.com";

            DomainRequest request = new DomainRequest { Id = domainId };

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _domainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            DmarcReadModel dmarcReadModel = new DmarcReadModel(new Domain.Domain(domainId, domainName), true, readmodel);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(request.Id)).Returns(Task.FromResult(dmarcReadModel));

            IActionResult result = await _domainStatusController.GetDmarcReadModel(request);

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<string>());

            string dmarcReadModelString = objectResult.Value as string;
            Assert.That(dmarcReadModelString, Is.EqualTo(readmodel));

            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<int>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDmarcReadModelForOrgDomainWithNoDmarcReturnOriginalReadModel()
        {
            int domainId = 1;
            string readmodel = "{\"readModel\": \"Test\"}";
            string organisationalDomainName = "xyz.com";

            DomainRequest request = new DomainRequest { Id = domainId };

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _domainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            DmarcReadModel dmarcReadModel = new DmarcReadModel(new Domain.Domain(1, organisationalDomainName), false, readmodel);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(request.Id)).Returns(Task.FromResult(dmarcReadModel));

            OrganisationalDomain organisationalDomain = new OrganisationalDomain(organisationalDomainName, organisationalDomainName);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(organisationalDomainName)).Returns(Task.FromResult(organisationalDomain));

            IActionResult result = await _domainStatusController.GetDmarcReadModel(request);

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<string>());

            string dmarcReadModelString = objectResult.Value as string;
            Assert.That(dmarcReadModelString, Is.EqualTo(readmodel));

            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<int>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task GetDmarcReadModelForDomainWhenDomainDoesntHaveDmarcAndOrgDomainIsNullReturnsOriginalReadModel()
        {
            int domainId = 1;
            string readmodel = "{\"readModel\": \"Test\"}";
            string domainName = "abc.xyz.com";
            string organisationalDomainName = "xyz.com";

            DomainRequest request = new DomainRequest { Id = domainId };

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _domainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            DmarcReadModel dmarcReadModel = new DmarcReadModel(new Domain.Domain(1, domainName), false, readmodel);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(request.Id)).Returns(Task.FromResult(dmarcReadModel));

            OrganisationalDomain organisationalDomain = new OrganisationalDomain(organisationalDomainName, domainName);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(domainName)).Returns(Task.FromResult(organisationalDomain));

            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<string>._)).Returns(Task.FromResult((DmarcReadModel)null));

            IActionResult result = await _domainStatusController.GetDmarcReadModel(request);

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<string>());

            string dmarcReadModelString = objectResult.Value as string;
            Assert.That(dmarcReadModelString, Is.EqualTo(readmodel));

            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<int>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task GetDmarcReadModelForDomainWhenDomainDoesntHaveDmarcAndOrgDomainDoesReturnsOrgDomainResult()
        {
            int domainId = 1;
            string readmodel = "{\"readModel\": \"Test\"}";
            string orgDomainReadModel = "{\"readModel\": \"Test\"}";
            string domainName = "abc.xyz.com";
            string organisationalDomainName = "xyz.com";

            DomainRequest request = new DomainRequest { Id = domainId };

            ValidationResult validationResult = new ValidationResult(new List<ValidationFailure>());
            A.CallTo(() => _domainRequestValidator.ValidateAsync(request, CancellationToken.None)).Returns(Task.FromResult(validationResult));

            DmarcReadModel dmarcReadModel = new DmarcReadModel(new Domain.Domain(1, domainName), false, readmodel);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(request.Id)).Returns(Task.FromResult(dmarcReadModel));

            OrganisationalDomain organisationalDomain = new OrganisationalDomain(organisationalDomainName, domainName);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(domainName)).Returns(Task.FromResult(organisationalDomain));

            DmarcReadModel orgDomainDmarcReadModel = new DmarcReadModel(new Domain.Domain(1, domainName), false, orgDomainReadModel);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<string>._)).Returns(Task.FromResult(orgDomainDmarcReadModel));

            IActionResult result = await _domainStatusController.GetDmarcReadModel(request);

            ObjectResult objectResult = result as ObjectResult;
            Assert.That(objectResult.Value, Is.TypeOf<string>());

            string dmarcReadModelString = objectResult.Value as string;
            Assert.That(dmarcReadModelString, Does.Contain("\"readModel\": \"Test\""));
            Assert.That(dmarcReadModelString, Does.Contain("inheritedFrom"));

            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<int>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _domainStatusDao.GetDmarcReadModel(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
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
    }
}
