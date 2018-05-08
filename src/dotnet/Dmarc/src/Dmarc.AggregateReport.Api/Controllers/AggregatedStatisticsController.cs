using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Dao.Aggregated;
using Dmarc.AggregateReport.Api.Dao.Domain;
using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dmarc.AggregateReport.Api.Controllers
{
    [Route("api/aggregatereport/aggregated")]
    public class AggregatedStatisticsController : DateRangeDomainController
    {
        private readonly IAggregatedStatisticsDao _aggregatedStatisticsDao;

        public AggregatedStatisticsController(IAggregatedStatisticsDao aggregatedStatisticsDao, 
            IDomainsDao domainsDao, 
            IValidator<DateRangeDomainRequest> dateRangeDomainValidator, 
            ILogger<AggregatedStatisticsController> log)
            : base(domainsDao, dateRangeDomainValidator, log)
        {
            _aggregatedStatisticsDao = aggregatedStatisticsDao;
        }

        [HttpGet]
        [Route("headline")]
        public Task<IActionResult> GetHeadline(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest,
                _aggregatedStatisticsDao.GetAggregatedHeadlineStatisticsAsync);
        }

        [HttpGet]
        [Route("trust")]
        public Task<IActionResult> GetTrust(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest,
                _aggregatedStatisticsDao.GetAggregatedTrustStatisticsAsync);
        }

        [HttpGet]
        [Route("compliance")]
        public Task<IActionResult> GetCompliance(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest,
                _aggregatedStatisticsDao.GetAggregatedComplianceStatisticsAsync);
        }

        [HttpGet]
        [Route("disposition")]
        public Task<IActionResult> GetDisposition(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest,
                _aggregatedStatisticsDao.GetAggregatedDispositionStatisticsAsync);
        }
    }
}
