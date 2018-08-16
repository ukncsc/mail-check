using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class AllEntitiesSearchRequestValidator : AbstractValidator<AllEntitiesSearchRequest>
    {
        public AllEntitiesSearchRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(r => r.Search)
                .Matches(@"\A[^\r\n$<>%;/\\]{0,50}\z")
                .WithMessage("A search cannot contain special characters.");

            RuleFor(r => r.Limit)
                .GreaterThanOrEqualTo(1)
                .WithMessage("A limit must be greater than zero.")
                .LessThanOrEqualTo(200)
                .WithMessage("A limit must be 200 or less.");
        }
    }
}
