using Dmarc.Metrics.Api.Domain;
using FluentValidation;

namespace Dmarc.Metrics.Api.Validation
{
    public class MetricsDateRangeValidator : AbstractValidator<MetricsDateRange>
    {
        public MetricsDateRangeValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(dr => dr.Start)
                .NotNull()
                .WithMessage("A valid start date must be provided.")
                .LessThan(dr => dr.End)
                .WithMessage("A start date cannot be after the end date.");

            RuleFor(dr => dr.End)
                .NotNull()
                .WithMessage("A valid end date must be provided.");
        }
    }
}
