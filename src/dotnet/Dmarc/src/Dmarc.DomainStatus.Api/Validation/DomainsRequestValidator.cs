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
                .GreaterThan(0);

            RuleFor(dr => dr.PageSize)
                .NotNull()
                .GreaterThan(0)
                .LessThanOrEqualTo(200);

            RuleFor(dr => dr.Search)
                .Length(0, 50)
                .Matches("^[a-zA-Z0-9.-]*$");
        }
    }
}