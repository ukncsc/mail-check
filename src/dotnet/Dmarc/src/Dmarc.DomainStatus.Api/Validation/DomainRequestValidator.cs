using Dmarc.DomainStatus.Api.Domain;
using FluentValidation;

namespace Dmarc.DomainStatus.Api.Validation
{
    internal class DomainRequestValidator : AbstractValidator<DomainRequest>
    {
        public DomainRequestValidator()
        {
            RuleFor(dr => dr.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("An ID must be greater than zero");
        }
    }
}
