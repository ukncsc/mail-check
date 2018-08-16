using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal class DomainSearchRequestValidator : AbstractValidator<DomainSearchRequest>
    {
        public DomainSearchRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(sp => sp.SearchPattern)
                .NotNull()
                .WithMessage("A search pattern must be provided.")
                .NotEmpty()
                .WithMessage("A search pattern must not be empty.")
                .Length(1, 50)
                .WithMessage("A search pattern must be between 1 and 50 characters.")
                .Matches("^[a-zA-Z0-9.-]+$")
                .WithMessage("A search pattern must not contain special characters");
        }
    }
}
