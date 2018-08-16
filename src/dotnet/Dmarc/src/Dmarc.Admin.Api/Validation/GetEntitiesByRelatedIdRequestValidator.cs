using Dmarc.Admin.Api.Domain;
using FluentValidation;

namespace Dmarc.Admin.Api.Validation
{
    public class GetEntitiesByRelatedIdRequestValidator : AbstractValidator<GetEntitiesByRelatedIdRequest>
    {
        public GetEntitiesByRelatedIdRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(r => r.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("An ID must be greater than zero.");

            RuleFor(dr => dr.Search)
                .Matches(@"\A[^\r\n$<>%;/\\]{0,50}\z")
                .WithMessage("A search must not contain special characters.");

            RuleFor(r => r.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("A page must be greater than zero.");

            RuleFor(r => r.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("A page size must be greater than 1.")
                .LessThanOrEqualTo(200)
                .WithMessage("A page size must be 200 or less.");
        }
    }
}
