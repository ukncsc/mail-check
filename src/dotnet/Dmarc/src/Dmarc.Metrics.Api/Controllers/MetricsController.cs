using Dmarc.Metrics.Api.Dao;
using Dmarc.Metrics.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dmarc.Metrics.Api.Controllers
{
    [Route("api/metrics")]
    public class MetricsController : Controller
    {
        private readonly IMetricsDao _metricsDao;

        public MetricsController(IMetricsDao metricsDao)
        {
            _metricsDao = metricsDao;
        }

        [HttpGet]
        [Route("{start}/{end}")]
        public async Task<IActionResult> GetMetrics(MetricsDateRange dateRange)
        {
            return new ObjectResult(await _metricsDao.GetMetrics(dateRange.Start, dateRange.End));
        }
    }
}
