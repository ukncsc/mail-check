using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Dmarc.AggregateReport.Api.Handlers;
using Dmarc.AggregateReport.Api.Handlers.Factory;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.

[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Dmarc.AggregateReport.Api
{
    public class AggregateReportApi
    {
        private readonly IGetAggregatedHeadlineStatisticsRequestHandler _getAggregatedHeadLineStatisticsRequestHandler;
        private readonly IGetAggregatedTrustStatisticsRequestHandler _getAggregatedTrustStatisticsRequestHandler;
        private readonly IGetAggregatedComplianceStatisticsRequestHandler _getAggregatedComplianceStatisticsRequestHandler;
        private readonly IGetAggregatedDispositionStatisticsRequestHandler _getAggregatedDispositionStatisticsRequestHandler;
        private readonly IGetMatchingDomainsRequestHandler _getDomainSearchRequestHandler;
        private readonly IGetDailyHeadlineStatisticsRequestHandler _getDailyHeadLineStatisticsRequestHandler;
        private readonly IGetDailyTrustStatisticsRequestHandler _getDailyTrustStatisticsRequestHandler;
        private readonly IGetDailyComplianceStatisticsRequestHandler _getDailyComplianceStatisticsRequestHandler;
        private readonly IGetDailyDispositionStatisticsRequestHandler _getDailyDispositionStatisticsRequestHandler;

        public AggregateReportApi()
        {
            _getAggregatedHeadLineStatisticsRequestHandler = RequestHandlerFactory.Create<IGetAggregatedHeadlineStatisticsRequestHandler>();
            _getAggregatedTrustStatisticsRequestHandler = RequestHandlerFactory.Create<IGetAggregatedTrustStatisticsRequestHandler>();
            _getAggregatedComplianceStatisticsRequestHandler = RequestHandlerFactory.Create<IGetAggregatedComplianceStatisticsRequestHandler>();
            _getAggregatedDispositionStatisticsRequestHandler = RequestHandlerFactory.Create<IGetAggregatedDispositionStatisticsRequestHandler>();
            _getDailyHeadLineStatisticsRequestHandler = RequestHandlerFactory.Create<IGetDailyHeadlineStatisticsRequestHandler>();
            _getDailyTrustStatisticsRequestHandler = RequestHandlerFactory.Create<IGetDailyTrustStatisticsRequestHandler>();
            _getDailyComplianceStatisticsRequestHandler = RequestHandlerFactory.Create<IGetDailyComplianceStatisticsRequestHandler>();
            _getDailyDispositionStatisticsRequestHandler = RequestHandlerFactory.Create<IGetDailyDispositionStatisticsRequestHandler>();
            _getDomainSearchRequestHandler = RequestHandlerFactory.Create<IGetMatchingDomainsRequestHandler>();
        }

        public Task<APIGatewayProxyResponse> GetAggregatedHeadlineStatistics(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _getAggregatedHeadLineStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetAggregatedTrustStatistics(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _getAggregatedTrustStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetAggregatedComplianceStatistics(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return _getAggregatedComplianceStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetAggregatedDispositionStatistics(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return _getAggregatedDispositionStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetDailyHeadlineStatistics(
           APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _getDailyHeadLineStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetDailyTrustStatistics(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _getDailyTrustStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetDailyComplianceStatistics(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return _getDailyComplianceStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetDailyDispositionStatistics(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return _getDailyDispositionStatisticsRequestHandler.ProcessRequest(request, context);
        }

        public Task<APIGatewayProxyResponse> GetMatchingDomains(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            return _getDomainSearchRequestHandler.ProcessRequest(request, context);
        }
    }
}
