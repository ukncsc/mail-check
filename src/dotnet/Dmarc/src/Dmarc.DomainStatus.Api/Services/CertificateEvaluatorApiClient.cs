using Dmarc.DomainStatus.Api.Config;
using Dmarc.DomainStatus.Api.Domain;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api.Services
{
    public interface ICertificateEvaluatorApi
    {
        Task<List<DomainSecurityInfo>> UpdateTlsWithCertificateEvaluatorStatus(List<DomainSecurityInfo> domainSecurityInfos);
    }

    public class CertificateEvaluatorApiClient : ICertificateEvaluatorApi
    {
        private readonly IDomainStatusApiConfig _config;
        private readonly ILogger<CertificateEvaluatorApiClient> _log;

        public CertificateEvaluatorApiClient(IDomainStatusApiConfig config, ILogger<CertificateEvaluatorApiClient> log)
        {
            _config = config;
            _log = log;
        }

        public async Task<List<DomainSecurityInfo>> UpdateTlsWithCertificateEvaluatorStatus(List<DomainSecurityInfo> domainSecurityInfos)
        {
            string path = "domains";
            List<CertificateEvaluatorApiResponse> response = null;

            try
            {
                response = await _config.CertificateEvaluatorApiEndpoint
                    .AppendPathSegment(path)
                    .PostJsonAsync(new CertificateEvaluatorApiRequest(domainSecurityInfos.Select(_ => _.Domain.Name).ToList()))
                    .ReceiveJson<List<CertificateEvaluatorApiResponse>>();
            }
            catch (FlurlHttpException ex)
            {
                _log.LogDebug(ex.Message);
            }
            catch (UriFormatException)
            {
                _log.LogDebug($"Bad URI for Certificate Evaluator API. Got endpoint {_config.CertificateEvaluatorApiEndpoint} and path {path}.");
            }

            return response != null
                ? domainSecurityInfos.Select(_ => UpdateTlsStatus(response, _)).ToList()
                : domainSecurityInfos;
        }

        private DomainSecurityInfo UpdateTlsStatus(List<CertificateEvaluatorApiResponse> response, DomainSecurityInfo domainSecurityInfo)
        {
            CertificateEvaluatorApiResponse evaluatorResult = response.FirstOrDefault(_ => _.DomainName == domainSecurityInfo.Domain.Name);

            if (evaluatorResult == null)
            {
                return domainSecurityInfo;
            }

            int maxCertificateErrorSeverity = evaluatorResult.HostResults
                .Where(_ => _.HostName != null)
                .SelectMany(_ => _.Errors)
                .Select(_ => (int)_.ErrorType)
                .DefaultIfEmpty(0)
                .Max();

            return (int)domainSecurityInfo.TlsStatus >= maxCertificateErrorSeverity
                ? domainSecurityInfo
                : new DomainSecurityInfo(
                    domainSecurityInfo.Domain,
                    domainSecurityInfo.HasDmarc,
                    (Status)maxCertificateErrorSeverity,
                    domainSecurityInfo.DmarcStatus,
                    domainSecurityInfo.SpfStatus);
        }
    }
}
