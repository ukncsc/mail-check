using Dmarc.Metrics.Api.Domain;
using FluentValidation;

namespace Dmarc.Metrics.Api.Validation
{
    public class MetricsDateRangeValidator : AbstractValidator<MetricsDateRange>
    {
        public MetricsDateRangeValidator()
        {
            RuleFor(dr => dr.Start).NotNull().LessThan(dr => dr.End);
            RuleFor(dr => dr.End).NotNull();
        }
    }
}
