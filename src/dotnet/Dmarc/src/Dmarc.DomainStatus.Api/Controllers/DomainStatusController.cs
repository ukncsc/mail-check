using Dmarc.Common.Api.Utils;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.Common.Serialisation;
using Dmarc.DomainStatus.Api.Dao.DomainStatus;
using Dmarc.DomainStatus.Api.Dao.Permission;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Services;
using Dmarc.DomainStatus.Api.Util;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api.Controllers
{
    [Route("api/domainstatus")]
    public class DomainStatusController : Controller
    {
        private readonly IDomainStatusDao _domainStatusDao;
        private readonly IPermissionDao _permissionDao;
        private readonly IReverseDnsApi _reverseDnsApi;
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;
        private readonly IValidator<DomainRequest> _domainRequestValidator;
        private readonly IValidator<DateRangeDomainRequest> _dateRangeDomainRequestValidator;
        private readonly ILogger _log;

        public DomainStatusController(IDomainStatusDao domainStatusDao,
            IPermissionDao permissionDao,
            IOrganisationalDomainProvider organisationalDomainProvider,
            IReverseDnsApi reverseDnsApi,
            IValidator<DomainRequest> domainRequestValidator,
            IValidator<DomainsRequest> domainsRequestValidator,
            IValidator<DateRangeDomainRequest> dateRangeDomainRequestValidator,
            ILogger<DomainStatusController> log)
        {
            _domainStatusDao = domainStatusDao;
            _permissionDao = permissionDao;
            _organisationalDomainProvider = organisationalDomainProvider;
            _reverseDnsApi = reverseDnsApi;
            _domainRequestValidator = domainRequestValidator;
            _dateRangeDomainRequestValidator = dateRangeDomainRequestValidator;
            _log = log;
        }

        [HttpGet]
        [Route("domain/{id}")]
        public async Task<IActionResult> GetDomain(DomainRequest domainRequest)
        {
            ValidationResult validationResult = await _domainRequestValidator.ValidateAsync(domainRequest);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}");
                return BadRequest(validationResult.GetErrorString());
            }

            Domain.Domain domain = await _domainStatusDao.GetDomain(domainRequest.Id);
            if (domain == null)
            {
                _log.LogWarning($"Domain with id: { domainRequest.Id } not found");
                return NotFound();
            }

            return new ObjectResult(domain);
        }

        [HttpGet]
        [Route("domain/tls/{id}")]
        public async Task<IActionResult> GetDomainTlsEvaluatorResults(DomainRequest domainRequest)
        {
            ValidationResult validationResult = await _domainRequestValidator.ValidateAsync(domainRequest);

            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}");
                return BadRequest(validationResult.GetErrorString());
            }

            DomainTlsEvaluatorResults domainTlsEvaluatorResults = await _domainStatusDao.GetDomainTlsEvaluatorResults(domainRequest.Id);

            if (domainTlsEvaluatorResults == null)
            {
                _log.LogWarning($"Antispoofing for Domain with id: { domainRequest.Id } not found");
                return NotFound();
            }

            return new ObjectResult(domainTlsEvaluatorResults);
        }

        [HttpGet]
        [Route("domain/spf/{id}")]
        public async Task<IActionResult> GetSpfReadModel(DomainRequest domainRequest)
        {
            ValidationResult validationResult = await _domainRequestValidator.ValidateAsync(domainRequest);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}");
                return BadRequest(validationResult.GetErrorString());
            }

            string spf = await _domainStatusDao.GetSpfReadModel(domainRequest.Id);

            return new ObjectResult(spf);
        }

        [HttpGet]
        [Route("domain/dmarc/{id}")]
        public async Task<IActionResult> GetDmarcReadModel(DomainRequest domainRequest)
        {
            ValidationResult validationResult = await _domainRequestValidator.ValidateAsync(domainRequest);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}");
                return BadRequest(validationResult.GetErrorString());
            }

            DmarcReadModel dmarc = await _domainStatusDao.GetDmarcReadModel(domainRequest.Id);

            if (dmarc.HasDmarc)
            {
                return new ObjectResult(dmarc.Model);
            }

            OrganisationalDomain organisationalDomain = await _organisationalDomainProvider.GetOrganisationalDomain(dmarc.Domain.Name);

            if (organisationalDomain.IsOrgDomain)
            {
                return new ObjectResult(dmarc.Model);
            }

            DmarcReadModel organisationalDomainDmarcRecord =
                await _domainStatusDao.GetDmarcReadModel(organisationalDomain.OrgDomain);

            if (organisationalDomainDmarcRecord == null)
            {
                return new ObjectResult(dmarc.Model);
            }

            JObject readModel = JObject.Parse(organisationalDomainDmarcRecord.Model);

            readModel.AddFirst(new JProperty("inheritedFrom", JToken.FromObject(organisationalDomainDmarcRecord.Domain,
                new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() })));

            return new ObjectResult(readModel.ToString());
        }

        [HttpGet]
        [Route("domain/aggregate/{id}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetAggregateReportSummary(DateRangeDomainRequest request)
        {
            ValidationResult validationResult = await _dateRangeDomainRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}.");
                return BadRequest(validationResult.GetErrorString());
            }

            int? userId = User.GetId();
            if (!userId.HasValue)
            {
                _log.LogWarning("Forbid unknown user.");
                return Forbid();
            }

            DomainPermissions domainPermissions = await _permissionDao.GetPermissions(userId.Value, request.Id);

            if (!domainPermissions.DomainPermission)
            {
                _log.LogWarning($"Domain {request.Id} not found for user {userId}.");
                return NotFound();
            }

            if (!domainPermissions.AggregatePermission)
            {
                _log.LogWarning($"Forbid user {userId}");
                return Forbid();
            }

            Task<SortedDictionary<DateTime, AggregateSummaryItem>> getInfos = _domainStatusDao.GetAggregateReportSummary(request.Id, request.StartDate, request.EndDate);
            Task<int> getEmailCount = _domainStatusDao.GetAggregateReportTotalEmailCount(request.Id, request.StartDate, request.EndDate);

            await Task.WhenAll(getInfos, getEmailCount);

            SortedDictionary<DateTime, AggregateSummaryItem> infoResults = getInfos.Result;

            if (!infoResults.Any())
            {
                return new ObjectResult(null);
            }

            int days = request.EndDate.Subtract(request.StartDate).Days + 1;
            if (days > infoResults.Count)
            {
                List<DateTime> datesRange = Enumerable.Range(0, days).Select(_ => new DateTime(request.StartDate.Ticks).AddDays(_)).ToList();
                foreach (DateTime date in datesRange)
                {
                    AggregateSummaryItem value;
                    if (!infoResults.TryGetValue(date, out value))
                    {
                        _log.LogDebug($"Added empty entry for date {date}");
                        infoResults[date] = new AggregateSummaryItem(0, 0, 0, 0, 0);
                    }
                }
            }

            AggregateSummary aggregateSummary = new AggregateSummary(infoResults, getEmailCount.Result);

            return new ObjectResult(aggregateSummary);
        }

        [HttpGet]
        [Route("domain/aggregate-export/{id}/{date}")]
        public async Task<IActionResult> GetAggregateReportExport(int id, DateTime date)
        {
            int? userId = User.GetId();
            if (!userId.HasValue)
            {
                _log.LogWarning("Forbid unknown user.");
                return Forbid();
            }

            DomainPermissions domainPermissions = await _permissionDao.GetPermissions(userId.Value, id);

            if (!domainPermissions.DomainPermission)
            {
                _log.LogWarning($"Domain {id} not found for user {userId}.");
                return NotFound();
            }

            if (!domainPermissions.AggregatePermission)
            {
                _log.LogWarning($"Forbid user {userId}");
                return Forbid();
            }

            List<AggregateReportExportItem> export = await _reverseDnsApi.AddReverseDnsInfoToExport(
                await _domainStatusDao.GetAggregateReportExport(id, date), date);

            Stream stream = await CsvSerialiser.SerialiseAsync(export);

            return File(stream, "application/octet-stream", $"AggregateReportExport-{id}-{date:yyyy-MM-dd}.csv");
        }
    }
}
