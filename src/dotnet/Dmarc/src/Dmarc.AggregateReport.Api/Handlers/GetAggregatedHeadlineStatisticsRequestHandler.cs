using System.Threading.Tasks;
using Dmarc.AggregateReport.Api.Dao;
using Dmarc.AggregateReport.Api.Dao.Entities;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.AggregateReport.Api.Messages.Factory;
using Dmarc.AggregateReport.Api.Validation;
using Dmarc.Common.Logging;

namespace Dmarc.AggregateReport.Api.Handlers
{
    internal interface IGetAggregatedHeadlineStatisticsRequestHandler : IRequestHandler{}

    internal class GetAggregatedHeadlineStatisticsRequestHandler : DateRangeDomainRequestHandler, IGetAggregatedHeadlineStatisticsRequestHandler
    {
        public GetAggregatedHeadlineStatisticsRequestHandler(ILogger log,
            IValidator<DateRangeDomainRequest> dateRangeDomainRequestValidator,
            IDateRangeDomainRequestFactory dateRangeDomainRequestFactory,
            IAggregateReportApiDao aggregateReportApiDao)
            : base(log, dateRangeDomainRequestValidator, dateRangeDomainRequestFactory, aggregateReportApiDao)
        {
        }

        protected override async Task<Response> CreateInternalResponseAsync(
            DateRangeDomainRequest request)
        {
            AggregatedStatistics aggregatedStatistics = await AggregateReportApiDao
                .GetAggregatedHeadlineStatisticsAsync(request.BeginDateUtc.Value, request.EndDateUtc.Value,
                    request.DomainId);
            return new AggregatedStatisticsResponse(aggregatedStatistics.Values);
        }
    }
}