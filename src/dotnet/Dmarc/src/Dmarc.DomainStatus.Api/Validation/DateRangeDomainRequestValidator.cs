using Dmarc.DomainStatus.Api.Domain;
using FluentValidation;

namespace Dmarc.DomainStatus.Api.Validation
{
    public class DateRangeDomainRequestValidator : AbstractValidator<DateRangeDomainRequest>
    {
        public DateRangeDomainRequestValidator()
        {
            RuleFor(_ => _.Id).GreaterThanOrEqualTo(0);
            RuleFor(_ => _.EndDate).GreaterThanOrEqualTo(_ => _.StartDate);
        }
    }
}
