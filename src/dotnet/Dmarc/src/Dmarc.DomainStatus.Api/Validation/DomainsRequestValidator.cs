using Dmarc.DomainStatus.Api.Domain;
using FluentValidation;

namespace Dmarc.DomainStatus.Api.Validation
{
    internal class DomainsRequestValidator : AbstractValidator<DomainsRequest>
    {
        public DomainsRequestValidator()
        {
            RuleFor(dr => dr.Page)
                .NotNull()
                .WithMessage("A page must not be null.")
                .GreaterThan(0)
                .WithMessage("A page must be greater than zero.");

            RuleFor(dr => dr.PageSize)
                .NotNull()
                .WithMessage("A page size must not be null.")
                .GreaterThan(0)
                .WithMessage("A page size must be greater than zero.")
                .LessThanOrEqualTo(200)
                .WithMessage("A page size must be 200 or less.");

            RuleFor(dr => dr.Search)
                .Length(0, 50)
                .WithMessage("A search must be between 1 and 50 characters.")
                .Matches("^[a-zA-Z0-9.-]*$")
                .WithMessage("A search must not contain special characters.");
        }
    }
}
