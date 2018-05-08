using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Dao.Domain;
using Dmarc.AggregateReport.Api.Dao.Sender;
using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dmarc.AggregateReport.Api.Controllers
{
    [Route("api/aggregatereport/senders")]
    public class SenderStatisticsController : DateRangeDomainController
    {
        private readonly ISenderStatisticsDao _senderStatisticsDao;

        public SenderStatisticsController(ISenderStatisticsDao senderStatisticsDao,
            IDomainsDao domainsDao,
            IValidator<DateRangeDomainRequest> dateRangeDomainValidator,
            ILogger<SenderStatisticsController> log)
            : base(domainsDao, dateRangeDomainValidator, log)
        {
            _senderStatisticsDao = senderStatisticsDao;
        }

        [HttpGet]
        [Route("trusted")]
        public Task<IActionResult> GetTrustedSenders(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _senderStatisticsDao.GetTrustedSenderStatistics);
        }

        [HttpGet]
        [Route("dkimnospf")]
        public Task<IActionResult> GetDkimNoSpfSenders(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _senderStatisticsDao.GetDkimNoSpfSenderStatistics);
        }

        [HttpGet]
        [Route("spfnodkim")]
        public Task<IActionResult> GetSpfNoDkimSenders(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _senderStatisticsDao.GetSpfNoDkimSenderStatistics);
        }

        [HttpGet]
        [Route("untrusted")]
        public Task<IActionResult> GetUntrustedSenders(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _senderStatisticsDao.GetUntrustedSenderStatistics);
        }
    }
}
