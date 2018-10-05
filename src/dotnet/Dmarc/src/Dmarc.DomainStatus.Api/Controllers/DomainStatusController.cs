using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Api.Domain;
using Dmarc.Common.Api.Utils;
using Dmarc.Common.Linq;
using Dmarc.Common.Serialisation;
using Dmarc.DomainStatus.Api.Dao.DomainStatus;
using Dmarc.DomainStatus.Api.Dao.Permission;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dmarc.DomainStatus.Api.Controllers
{
    [Route("api/domainstatus")]
    public class DomainStatusController : Controller
    {
        private readonly IDomainStatusDao _domainStatusDao;
        private readonly IPermissionDao _permissionDao;
        private readonly IReverseDnsApi _reverseDnsApi;
        private readonly IValidator<DomainRequest> _domainRequestValidator;
        private readonly IValidator<DateRangeDomainRequest> _dateRangeDomainRequestValidator;
        private readonly ILogger _log;

        public DomainStatusController(IDomainStatusDao domainStatusDao,
            IPermissionDao permissionDao,
            IReverseDnsApi reverseDnsApi,
            IValidator<DomainRequest> domainRequestValidator,
            IValidator<DateRangeDomainRequest> dateRangeDomainRequestValidator,
            ILogger<DomainStatusController> log)
        {
            _domainStatusDao = domainStatusDao;
            _permissionDao = permissionDao;
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
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            Domain.Domain domain = await _domainStatusDao.GetDomain(domainRequest.Id);
            if (domain == null)
            {
                _log.LogWarning($"Domain with id: { domainRequest.Id } not found");
                return NotFound(new ErrorResponse("Domain not found.", ErrorStatus.Information));
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
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            DomainTlsEvaluatorResults domainTlsEvaluatorResults = await _domainStatusDao.GetDomainTlsEvaluatorResults(domainRequest.Id);

            if (domainTlsEvaluatorResults == null)
            {
                _log.LogWarning($"Antispoofing for Domain with id: { domainRequest.Id } not found");
                return NotFound(new ErrorResponse($"No domain found for ID {domainRequest.Id}."));
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
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            string spf = await _domainStatusDao.GetSpfReadModel(domainRequest.Id);

            if (spf == null)
            {
                Domain.Domain domain = await _domainStatusDao.GetDomain(domainRequest.Id);
                if (domain == null)
                {
                    return NotFound(new ErrorResponse($"No domain found for ID {domainRequest.Id}."));
                }
                return new ObjectResult(new { records = (List<string>)null, pending = true });
            }

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
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
            }

            string dmarc = await _domainStatusDao.GetDmarcReadModel(domainRequest.Id);

            if (dmarc == null)
            {
                Domain.Domain domain = await _domainStatusDao.GetDomain(domainRequest.Id);
                if (domain == null)
                {
                    return NotFound(new ErrorResponse($"No domain found for ID {domainRequest.Id}."));
                }

                return new ObjectResult(new { records = (List<string>)null, pending = true });
            }

            return new ObjectResult(dmarc);
        }

        [HttpGet]
        [Route("domain/aggregate/{id}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetAggregateReportSummary(DateRangeDomainRequest request)
        {
            ValidationResult validationResult = await _dateRangeDomainRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _log.LogWarning($"Bad request: {validationResult.GetErrorString()}.");
                return BadRequest(new ErrorResponse(validationResult.GetErrorString()));
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
                return NotFound(new ErrorResponse($"No domain found for ID {request.Id}."));
            }

            if (!domainPermissions.AggregatePermission)
            {
                _log.LogWarning($"Forbid user {userId}");
                return Forbid();
            }

            bool.TryParse(HttpContext.Request.Query["includeSubdomains"].ToString(), out bool includeSubdomains);

            Task<SortedDictionary<DateTime, AggregateSummaryItem>> getInfos = _domainStatusDao.GetAggregateReportSummary(request.Id, request.StartDate, request.EndDate, includeSubdomains);
            Task<int> getEmailCount = _domainStatusDao.GetAggregateReportTotalEmailCount(request.Id, request.StartDate, request.EndDate, includeSubdomains);

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
                    if (!infoResults.TryGetValue(date, out AggregateSummaryItem value))
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
        [Route("domain/aggregate-export/{id}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetAggregateReportExport(int id, DateTime startDate, DateTime endDate)
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
                return NotFound(new ErrorResponse($"No domain found for ID {id}."));
            }

            if (!domainPermissions.AggregatePermission)
            {
                _log.LogWarning($"Forbid user {userId}");
                return Forbid();
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            List<AggregateReportExportItem> results =
                await _domainStatusDao.GetAggregateReportExport(id, startDate, endDate);

            _log.LogInformation($"Took {stopwatch.Elapsed.Seconds} seconds to get aggregate data for domain ID {id}.");

            stopwatch.Restart();

            List<AggregateReportExportItem> export = results
                .GroupBy(_ => _.EffectiveDate)
                .ToDictionary(_ => _.Key, _ => _.ToList())
                .Select(_ => _reverseDnsApi.AddReverseDnsInfoToExport(_.Value, _.Key))
                .Batch(10)
                .Select(async _ => await Task.WhenAll(_))
                .SelectMany(_ => _.Result.SelectMany(x => x))
                .OrderByDescending(_ => _.EffectiveDate)
                .ToList();

            _log.LogInformation($"Took {stopwatch.Elapsed.Seconds} seconds to get reverse dns data for domain ID {id}.");

            stopwatch.Stop();

            Stream stream = await CsvSerialiser.SerialiseAsync(export);

            return File(stream,
                "application/octet-stream",
                $"AggregateReportExport_{id}_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}.csv");
        }
    }
}
