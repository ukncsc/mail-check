using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Dao.Daily;
using Dmarc.AggregateReport.Api.Dao.Domain;
using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dmarc.AggregateReport.Api.Controllers
{
    [Route("api/aggregatereport/daily")]
    public class DailyStatisticsController : DateRangeDomainController
    {
        private readonly IDailyStatisticsDao _dailyStatisticsDao;

        public DailyStatisticsController(IDailyStatisticsDao dailyStatisticsDao, 
            IDomainsDao domainsDao,
            IValidator<DateRangeDomainRequest> dateRangeDomainValidator,
            ILogger<DailyStatisticsController> log
            ) : base(domainsDao, dateRangeDomainValidator, log)
        {
            _dailyStatisticsDao = dailyStatisticsDao;
        }

        [HttpGet]
        [Route("trust")]
        public Task<IActionResult> GetTrust(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _dailyStatisticsDao.GetDailyTrustStatisticsAsync);
        }

        [HttpGet]
        [Route("compliance")]
        public Task<IActionResult> GetCompliance(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _dailyStatisticsDao.GetDailyComplianceStatisticsAsync);
        }

        [HttpGet]
        [Route("disposition")]
        public  Task<IActionResult> GetDisposition(DateRangeDomainRequest dateRangeDomainRequest)
        {
            return GetDateRangeDomainResult(dateRangeDomainRequest, _dailyStatisticsDao.GetDailyDispositionStatisticsAsync);
        }
    }
}
