using Dmarc.DomainStatus.Api.Config;
using Dmarc.DomainStatus.Api.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dmarc.DomainStatus.Api.Services
{
    public interface IReverseDnsApi
    {
        Task<List<AggregateReportExportItem>> AddReverseDnsInfoToExport(List<AggregateReportExportItem> export, DateTime date);
    }

    public class ReverseDnsApi : IReverseDnsApi
    {
        private readonly HttpClient _client;
        private readonly IReverseDnsApiConfig _config;
        private readonly ILogger<ReverseDnsApi> _log;

        public ReverseDnsApi(HttpClient client, IReverseDnsApiConfig config, ILogger<ReverseDnsApi> log)
        {
            _client = client;
            _config = config;
            _log = log;
        }

        public async Task<List<AggregateReportExportItem>> AddReverseDnsInfoToExport(List<AggregateReportExportItem> export, DateTime date)
        {
            Uri uri;
            if (Uri.TryCreate(_config.Endpoint, UriKind.Absolute, out uri))
            {
                HttpResponseMessage response = await _client.PostAsync(
                    new Uri(uri, "/info"),
                    new StringContent(JsonConvert.SerializeObject(new ReverseDnsInfoApiRequest(export.Select(_ => _.SourceIp).ToList(), date))));

                if (response.IsSuccessStatusCode)
                {
                    List<ReverseDnsInfoApiResponse> reverseDnsInfoResponses = JsonConvert.DeserializeObject<List<ReverseDnsInfoApiResponse>>(
                        await response.Content.ReadAsStringAsync());

                    return export.Select(_ => AddPtrInfo(reverseDnsInfoResponses, _)).ToList();
                }

                _log.LogWarning($"Request to Reverse DNS API failed with status code {response.StatusCode}");
            }
            else
            {
                _log.LogWarning($"Invalid URI provided for Reverse DNS API, received {_config.Endpoint}");
            }

            return export;
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
