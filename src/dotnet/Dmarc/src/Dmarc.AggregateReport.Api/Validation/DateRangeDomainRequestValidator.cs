using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal class DateRangeDomainRequestValidator : AbstractValidator<DateRangeDomainRequest>
    {
        public DateRangeDomainRequestValidator()
        {
            RuleFor(dr => dr.BeginDateUtc).NotNull();
            RuleFor(dr => dr.EndDateUtc).NotNull();
        }
    }
}
