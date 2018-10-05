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
    public interface IReverseDnsApi
    {
        Task<List<AggregateReportExportItem>> AddReverseDnsInfoToExport(List<AggregateReportExportItem> export, DateTime date);
    }

    public class ReverseDnsApiClient : IReverseDnsApi
    {
        private readonly IDomainStatusApiConfig _config;
        private readonly ILogger<ReverseDnsApiClient> _log;

        public ReverseDnsApiClient(IDomainStatusApiConfig config, ILogger<ReverseDnsApiClient> log)
        {
            _config = config;
            _log = log;
        }

        public async Task<List<AggregateReportExportItem>> AddReverseDnsInfoToExport(List<AggregateReportExportItem> export, DateTime date)
        {
            string path = "info";
            List<ReverseDnsInfoApiResponse> response = null;

            try
            {
                response = await _config.ReverseDnsApiEndpoint
                    .AppendPathSegment(path)
                    .PostJsonAsync(new ReverseDnsInfoApiRequest(export.Select(_ => _.SourceIp).ToList(), date))
                    .ReceiveJson<List<ReverseDnsInfoApiResponse>>();
            }
            catch (FlurlHttpException ex)
            {
                _log.LogDebug(ex.Message);
            }
            catch (UriFormatException)
            {
                _log.LogDebug($"Bad URI for Reverse DNS API. Got endpoint {_config.ReverseDnsApiEndpoint} and path {path}.");
            }

            return response != null
                ? export.Select(_ => AddPtrInfo(response, _)).ToList()
                : export;
        }

        private AggregateReportExportItem AddPtrInfo(List<ReverseDnsInfoApiResponse> reverseDnsInfoResponses, AggregateReportExportItem _)
        {
            string ptrStatus = GetPtrStatus(reverseDnsInfoResponses.FirstOrDefault(x => x.IpAddress == _.SourceIp));

            return new AggregateReportExportItem(_.HeaderFrom, _.SourceIp, ptrStatus, _.Count, _.Spf, _.Dkim, _.Disposition, _.OrgName, _.EffectiveDate);
        }

        private string GetPtrStatus(ReverseDnsInfoApiResponse reverseDnsInfo)
        {
            if (reverseDnsInfo == null || !reverseDnsInfo.DnsResponses.Any())
            {
                return "Unknown";
            }

            if (!reverseDnsInfo.ForwardLookupMatches.Any())
            {
                return "Mismatch";
            }

            return string.Join(" or ", reverseDnsInfo.ForwardLookupMatches);
        }
    }
}
