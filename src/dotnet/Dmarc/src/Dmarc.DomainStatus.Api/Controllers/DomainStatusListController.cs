using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dmarc.Common.Api.Utils;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DomainStatus.Api.Dao.DomainStatusList;
using Dmarc.DomainStatus.Api.Domain;
using Dmarc.DomainStatus.Api.Util;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dmarc.DomainStatus.Api.Controllers
{
    [Route("api/domainstatus")]
    public class DomainStatusListController : Controller
    {
        private readonly IDomainStatusListDao _domainStatusListDao;
        private readonly IOrganisationalDomainProvider _organisationalDomainProvider;
        private readonly IValidator<DomainsRequest> _domainsRequestValidator;
        private readonly ILogger<DomainStatusListController> _log;

        public DomainStatusListController(IDomainStatusListDao domainStatusListDao,
            IOrganisationalDomainProvider organisationalDomainProvider,
            IValidator<DomainsRequest> domainsRequestValidator,
            ILogger<DomainStatusListController> log)
        {
            _domainStatusListDao = domainStatusListDao;
            _organisationalDomainProvider = organisationalDomainProvider;
            _domainsRequestValidator = domainsRequestValidator;
            _log = log;
        }

        [HttpGet]
        [Route("domains_security")]
        public Task<IActionResult> GetDomainsSecurityInfo(DomainsRequest request)
        {
            Func<DomainsSecurityResponse, List<DomainSecurityInfo>, DomainsSecurityResponse> responseFactory = (response, items) => new DomainsSecurityResponse(items, response.DomainCount);
            return GetDomainSecurityInfoInternal(request, GetDomainsSecurityInfoResponse, responseFactory);
        }

        [HttpGet]
        [Route("domains_security/user")]
        public Task<IActionResult> GetDomainsSecurityInfoByUserId(DomainsRequest request)
        {
            Func<MyDomainsResponse, List<DomainSecurityInfo>, MyDomainsResponse> responseFactory = (response, items) => new MyDomainsResponse(items, response.DomainCount, response.UserDomainCount);
            return GetDomainSecurityInfoInternal(request, GetDomainsSecurityInfoResponseByUserId, responseFactory);
        }

        private async Task<DomainsSecurityResponse> GetDomainsSecurityInfoResponse(DomainsRequest request)
        {
            Task<List<DomainSecurityInfo>> getDomainSecurityInfoTask 
                = _domainStatusListDao.GetDomainsSecurityInfo(request.Page.Value, request.PageSize.Value, request.Search);

            Task<long> getDomainCountTask = _domainStatusListDao.GetDomainsCount(request.Search);

            await Task.WhenAll(getDomainSecurityInfoTask, getDomainCountTask);

            return new DomainsSecurityResponse(getDomainSecurityInfoTask.Result, getDomainCountTask.Result);
        }

        private async Task<MyDomainsResponse> GetDomainsSecurityInfoResponseByUserId(DomainsRequest request)
        {
            int? userId = User.GetId();

            if (!userId.HasValue)
            {
                return new MyDomainsResponse(new List<DomainSecurityInfo>(), 0, 0);
            }

            Task<List<DomainSecurityInfo>> getDomainSecurityInfoTask
                = _domainStatusListDao.GetDomainsSecurityInfoByUserId(userId.Value, request.Page.Value, request.PageSize.Value, request.Search);

            Task<long> getDomainCountTask = _domainStatusListDao.GetDomainsCountByUserId(userId.Value, request.Search);

            Task<long> getUserDomainCountTask = _domainStatusListDao.GetDomainsCountByUserId(userId.Value, string.Empty);

            await Task.WhenAll(getDomainSecurityInfoTask, getDomainCountTask, getUserDomainCountTask);

            return new MyDomainsResponse(getDomainSecurityInfoTask.Result, getDomainCountTask.Result, getUserDomainCountTask.Result);
        }

        private async Task<IActionResult> GetDomainSecurityInfoInternal<T>(DomainsRequest request,
            Func<DomainsRequest, Task<T>> getDomainSecurityInfoResponse,
            Func<T, List<DomainSecurityInfo>, T> updateResponseFactory) where T : DomainsSecurityResponse
        {
            ValidationResult validationResult = await _domainsRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errorString = validationResult.GetErrorString();
                _log.LogWarning($"Bad request: {errorString}");
                return BadRequest(new ValidationError(errorString));
            }

            T domainsSecurityResponse = 
                await getDomainSecurityInfoResponse(request);

            List<DomainSecurityInfo> domainSecurityInfoWithoutDmarcRecord =
                domainsSecurityResponse.DomainSecurityInfos.Where(_ => !_.HasDmarc).ToList();

            if (!domainSecurityInfoWithoutDmarcRecord.Any())
            {
                return new ObjectResult(domainsSecurityResponse);
            }

            Dictionary<string, string> nonOrgDomainsWithoutDmarcRecordForOrgDomains =
                await GetOrganisationalDomains(domainSecurityInfoWithoutDmarcRecord);

            if (!nonOrgDomainsWithoutDmarcRecordForOrgDomains.Any())
            {
                return new ObjectResult(domainsSecurityResponse);
            }

            List<string> organisationalDomains = nonOrgDomainsWithoutDmarcRecordForOrgDomains.Values.Distinct().ToList();
             
            List<DomainSecurityInfo> orgDomainSecurityInfos = await _domainStatusListDao
                .GetDomainsSecurityInfoByDomainNames(organisationalDomains);

            List<DomainSecurityInfo> updatedDomainSecurityInfos = Merge(domainsSecurityResponse.DomainSecurityInfos, orgDomainSecurityInfos,
                nonOrgDomainsWithoutDmarcRecordForOrgDomains);

            return new ObjectResult(updateResponseFactory(domainsSecurityResponse, updatedDomainSecurityInfos)); 
        }

        private async Task<Dictionary<string, string>> GetOrganisationalDomains(
            List<DomainSecurityInfo> domainSecurityInfosWithoutDmarc)
        {
            List<Task<OrganisationalDomain>> orgDomainTasks = domainSecurityInfosWithoutDmarc
                .Select(_ => _organisationalDomainProvider.GetOrganisationalDomain(_.Domain.Name)).ToList();

            await Task.WhenAll(orgDomainTasks);

            return orgDomainTasks.Select(_ => _.Result).Where(_ => !_.IsOrgDomain).ToDictionary(_ => _.Domain, _ => _.OrgDomain);
        }

        private List<DomainSecurityInfo> Merge(List<DomainSecurityInfo> existing, List<DomainSecurityInfo> updated, Dictionary<string, string> updatedToExistingLookup)
        {
            Dictionary<string, DomainSecurityInfo> existingDictionary = existing.ToDictionary(_ => _.Domain.Name);
            Dictionary<string, DomainSecurityInfo> updatedDictionary = updated.ToDictionary(_ => _.Domain.Name);

            foreach (KeyValuePair<string, string> subDomainOrgDomainPair in updatedToExistingLookup)
            {
                DomainSecurityInfo updatedDomainSecurityInfo;
                if (updatedDictionary.TryGetValue(subDomainOrgDomainPair.Value, out updatedDomainSecurityInfo) && updatedDomainSecurityInfo.HasDmarc)
                {
                    existingDictionary[subDomainOrgDomainPair.Key] = 
                        CreateOrganistionDomainSecurityInfo(existingDictionary[subDomainOrgDomainPair.Key], updatedDomainSecurityInfo);
                }
            }

            return existingDictionary.Values.ToList();
        }

        private DomainSecurityInfo CreateOrganistionDomainSecurityInfo(DomainSecurityInfo domainSecurityInfo,
            DomainSecurityInfo orgDomainSecurityInfo)
        {
            return new DomainSecurityInfo(
                domainSecurityInfo.Domain,
                orgDomainSecurityInfo.HasDmarc,
                domainSecurityInfo.TlsStatus,
                orgDomainSecurityInfo.DmarcStatus,
                domainSecurityInfo.SpfStatus);
        }

        private class ValidationError
        {
            public ValidationError(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }
    }
}