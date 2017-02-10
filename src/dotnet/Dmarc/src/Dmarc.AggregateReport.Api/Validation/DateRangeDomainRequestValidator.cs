using Dmarc.AggregateReport.Api.Messages;
using FluentValidation;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal class DateRangeDomainRequestValidator : AbstractValidator<DateRangeDomainRequest>, IValidator<DateRangeDomainRequest>
    {
        public DateRangeDomainRequestValidator()
        {
            RuleFor(dr => dr.BeginDateUtc).NotNull();
            RuleFor(dr => dr.EndDateUtc).NotNull();
        }
    }
}
