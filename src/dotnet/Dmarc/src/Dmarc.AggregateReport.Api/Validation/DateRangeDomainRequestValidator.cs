using Dmarc.AggregateReport.Api.Domain;
using FluentValidation;

namespace Dmarc.AggregateReport.Api.Validation
{
    internal class DateRangeDomainRequestValidator : AbstractValidator<DateRangeDomainRequest>
    {
        public DateRangeDomainRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(dr => dr.BeginDateUtc)
                .NotNull()
                .WithMessage("A valid begin date must be provided.");

            RuleFor(dr => dr.EndDateUtc)
                .NotNull()
                .WithMessage("A valid end date must be provided.");
        }
    }
}
