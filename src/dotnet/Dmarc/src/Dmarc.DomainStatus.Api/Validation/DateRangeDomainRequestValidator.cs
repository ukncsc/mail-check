using Dmarc.DomainStatus.Api.Domain;
using FluentValidation;

namespace Dmarc.DomainStatus.Api.Validation
{
    public class DateRangeDomainRequestValidator : AbstractValidator<DateRangeDomainRequest>
    {
        public DateRangeDomainRequestValidator()
        {
            RuleFor(_ => _.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("An id must be greater than zero.");

            RuleFor(_ => _.EndDate)
                .GreaterThanOrEqualTo(_ => _.StartDate)
                .WithMessage("An end date must be greater than the start date.");
        }
    }
}
