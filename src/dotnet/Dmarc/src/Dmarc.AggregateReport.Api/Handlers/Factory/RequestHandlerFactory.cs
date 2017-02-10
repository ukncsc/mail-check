using System;
using Dmarc.AggregateReport.Api.Dao;
using Dmarc.AggregateReport.Api.Logger;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.AggregateReport.Api.Messages.Factory;
using Dmarc.AggregateReport.Api.Validation;
using Dmarc.Common.Data;
using Dmarc.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.AggregateReport.Api.Handlers.Factory
{
    internal class RequestHandlerFactory
    {
        public static T Create<T>()
            where T : IRequestHandler
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<ILogger, LambdaLoggerAdaptor>()
                .AddTransient<IDateRangeDomainRequestFactory, DateRangeDomainRequestFactory>()
                .AddTransient<IDomainSearchRequestFactory, DomainSearchRequestFactory>()
                .AddTransient<IValidator<DateRangeDomainRequest>, DateRangeDomainRequestValidator>()
                .AddTransient<IValidator<DomainSearchRequest>, DomainSearchRequestValidator>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IAggregateReportApiDao, AggregateReportApiDao>()
                .AddTransient<IGetAggregatedHeadlineStatisticsRequestHandler, GetAggregatedHeadlineStatisticsRequestHandler>()
                .AddTransient<IGetAggregatedTrustStatisticsRequestHandler, GetAggregatedTrustStatisticsRequestHandler>()
                .AddTransient<IGetAggregatedComplianceStatisticsRequestHandler, GetAggregatedComplianceStatisticsRequestHandler>()
                .AddTransient<IGetAggregatedDispositionStatisticsRequestHandler, GetAggregatedDispositionStatisticsRequestHandler>()
                .AddTransient<IGetDailyHeadlineStatisticsRequestHandler, GetDailyHeadlineStatisticsRequestHandler>()
                .AddTransient<IGetDailyTrustStatisticsRequestHandler, GetDailyTrustStatisticsRequestHandler>()
                .AddTransient<IGetDailyComplianceStatisticsRequestHandler, GetDailyComplianceStatisticsRequestHandler>()
                .AddTransient<IGetDailyDispositionStatisticsRequestHandler, GetDailyDispositionStatisticsRequestHandler>()
                .AddTransient<IGetMatchingDomainsRequestHandler, GetMatchingDomainsRequestHandler>()
                .BuildServiceProvider();

            return serviceProvider.GetService<T>();
        }
    }
}
