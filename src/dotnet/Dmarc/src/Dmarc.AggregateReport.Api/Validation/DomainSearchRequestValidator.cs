using Dmarc.AggregateReport.Api.Messages;
using FluentValidation;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal class DomainSearchRequestValidator : AbstractValidator<DomainSearchRequest>, IValidator<DomainSearchRequest>
    {
        public DomainSearchRequestValidator()
        {
            RuleFor(dsr => dsr.SearchPattern)
                .NotEmpty()
                .Length(1,50)
                .Matches("[a-z0-9.-]+");
        }
    }
}
