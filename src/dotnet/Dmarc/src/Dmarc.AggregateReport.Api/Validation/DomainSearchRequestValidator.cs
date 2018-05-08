using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal class DomainSearchRequestValidator : AbstractValidator<DomainSearchRequest>
    {
        public DomainSearchRequestValidator()
        {
            RuleFor(sp => sp.SearchPattern)
                .NotNull()
                .NotEmpty()
                .Length(1, 50)
                .Matches("^[a-zA-Z0-9.-]+$");
        }
    }
}