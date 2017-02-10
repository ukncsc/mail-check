using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Dmarc.AggregateReport.Api.Dao;
using Dmarc.AggregateReport.Api.Dao.Entities;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.AggregateReport.Api.Messages.Factory;
using Dmarc.AggregateReport.Api.Validation;
using Dmarc.Common.Logging;
using FluentValidation.Results;

namespace Dmarc.AggregateReport.Api.Handlers
{
    internal interface IGetMatchingDomainsRequestHandler : IRequestHandler { }

    internal class GetMatchingDomainsRequestHandler : AbstractRequestHandler<DomainSearchRequest, DomainSearchResponse>, IGetMatchingDomainsRequestHandler
    {
        private readonly IValidator<DomainSearchRequest> _domainSearchRequestValidator;
        private readonly IDomainSearchRequestFactory _domainSearchRequestFactory;
        private readonly IAggregateReportApiDao _aggregateReportApiDao;

        public GetMatchingDomainsRequestHandler(ILogger log,
            IValidator<DomainSearchRequest> domainSearchRequestValidator,
            IDomainSearchRequestFactory domainSearchRequestFactory,
            IAggregateReportApiDao aggregateReportApiDao) 
            : base(log)
        {
            _domainSearchRequestValidator = domainSearchRequestValidator;
            _domainSearchRequestFactory = domainSearchRequestFactory;
            _aggregateReportApiDao = aggregateReportApiDao;
        }

        protected override Task<ValidationResult> ValidateAsync(DomainSearchRequest request)
        {
            return _domainSearchRequestValidator.ValidateAsync(request);
        }

        protected override Task<DomainSearchRequest> CreateInternalRequestAsync(APIGatewayProxyRequest request)
        {
            return Task.FromResult(_domainSearchRequestFactory.Create(request));
        }

        protected override async Task<DomainSearchResponse> CreateInternalResponseAsync(DomainSearchRequest request)
        {
            MatchingDomains matchingDomains = await _aggregateReportApiDao.GetMatchingDomains(request.SearchPattern);
            return new DomainSearchResponse(matchingDomains);
        }
    }
}
