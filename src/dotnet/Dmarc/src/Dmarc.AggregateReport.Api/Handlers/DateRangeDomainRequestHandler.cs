using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Dmarc.AggregateReport.Api.Dao;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.AggregateReport.Api.Messages.Factory;
using Dmarc.AggregateReport.Api.Validation;
using Dmarc.Common.Logging;
using FluentValidation.Results;

namespace Dmarc.AggregateReport.Api.Handlers
{
    internal abstract class DateRangeDomainRequestHandler :
        AbstractRequestHandler<DateRangeDomainRequest, Response>
    {
        private readonly IValidator<DateRangeDomainRequest> _dateRangeDomainRequestValidator;
        private readonly IDateRangeDomainRequestFactory _dateRangeDomainRequestFactory;
        protected readonly IAggregateReportApiDao AggregateReportApiDao;

        protected DateRangeDomainRequestHandler(ILogger log,
            IValidator<DateRangeDomainRequest> dateRangeDomainRequestValidator,
            IDateRangeDomainRequestFactory dateRangeDomainRequestFactory,
            IAggregateReportApiDao aggregateReportApiDao)
            : base(log)
        {
            _dateRangeDomainRequestValidator = dateRangeDomainRequestValidator;
            _dateRangeDomainRequestFactory = dateRangeDomainRequestFactory;
            AggregateReportApiDao = aggregateReportApiDao;
        }

        protected override Task<ValidationResult> ValidateAsync(DateRangeDomainRequest request)
        {
            return _dateRangeDomainRequestValidator.ValidateAsync(request);
        }

        protected override Task<DateRangeDomainRequest> CreateInternalRequestAsync(APIGatewayProxyRequest request)
        {
            return Task.FromResult(_dateRangeDomainRequestFactory.Create(request));
        }

        protected override Task<bool> ResourceExists(DateRangeDomainRequest request)
        {
            if (request.DomainId.HasValue)
            {
                return AggregateReportApiDao.DomainExists(request.DomainId.Value);
            }
            return base.ResourceExists(request);
        }
    }
}