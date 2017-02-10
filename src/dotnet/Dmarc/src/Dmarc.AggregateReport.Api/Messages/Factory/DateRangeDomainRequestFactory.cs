using System;
using Amazon.Lambda.APIGatewayEvents;
using Dmarc.AggregateReport.Api.Handlers.Util;

namespace Dmarc.AggregateReport.Api.Messages.Factory
{
    internal interface IDateRangeDomainRequestFactory
    {
        DateRangeDomainRequest Create(APIGatewayProxyRequest request);
    }

    internal class DateRangeDomainRequestFactory : IDateRangeDomainRequestFactory
    {
        public DateRangeDomainRequest Create(APIGatewayProxyRequest request)
        {
            DateTime? beginDateUtc = request.QueryStringParameters?.GetDateTime("beginDateUtc");
            DateTime? endDateUtc = request.QueryStringParameters?.GetDateTime("endDateUtc");
            int? domainId = request.QueryStringParameters?.GetInt("domainId");

            return new DateRangeDomainRequest(beginDateUtc, endDateUtc, domainId);
        }
    }
}