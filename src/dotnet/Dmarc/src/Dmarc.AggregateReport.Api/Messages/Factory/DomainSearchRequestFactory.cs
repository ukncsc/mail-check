using Amazon.Lambda.APIGatewayEvents;
using Dmarc.AggregateReport.Api.Handlers.Util;

namespace Dmarc.AggregateReport.Api.Messages.Factory
{
    internal interface IDomainSearchRequestFactory
    {
        DomainSearchRequest Create(APIGatewayProxyRequest request);
    }

    internal class DomainSearchRequestFactory : IDomainSearchRequestFactory
    {
        public DomainSearchRequest Create(APIGatewayProxyRequest request)
        {
            string searchPattern = request.QueryStringParameters?.GetString("searchPattern");
            return new DomainSearchRequest(searchPattern);
        }
    }
}
